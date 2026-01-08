namespace MediaRatingProject.Data.Stores
{
    using MediaRatingProject.Data.Media;
    using MediaRatingProject.Data.Ratings;
    using MediaRatingProject.Data.StoreInterfaces;
    using MediaRatingProject.Data.Users;
    using Npgsql;

    /// <summary>
    /// Class responsible for managing media data in the database.
    /// </summary>
    public class MediaStore: IMediaStore
    {
        private readonly string _connectionString;

        public MediaStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Creates a new media entry.
        /// </summary>
        /// <param name="media">Media data.</param>
        /// <returns>Boolean indicating whether the operation is successful.</returns>
        public bool CreateMedia(BaseMedia media)
        {
            if (media == null || string.IsNullOrWhiteSpace(media.Title) || string.IsNullOrWhiteSpace(media.MediaType))
                return false;

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    INSERT INTO media (title, description, media_type, release_year, age_restriction, genres, creator_id)
                    VALUES (@title, @description, @mediaType, @releaseYear, @ageRestriction, @genres, @creatorId)
                    RETURNING id;
                 ";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("title", media.Title);
                cmd.Parameters.AddWithValue("description", (object)media.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("mediaType", media.MediaType);
                cmd.Parameters.AddWithValue("releaseYear", media.ReleaseYear.HasValue ? (object)media.ReleaseYear.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("ageRestriction", media.AgeRestriction.HasValue ? (object)media.AgeRestriction.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("genres", media.Genres != null ? media.Genres.ToArray() : new string[] { });
                cmd.Parameters.AddWithValue("creatorId", media.UserId);

                // Execute the insert and get the generated ID
                media.Id = (int)cmd.ExecuteScalar();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating media: {ex.Message}");
                return false;
            }

        }

        /// <summary>
        /// Gets a media by its ID, including ratings and likes.
        /// </summary>
        /// <param name="id">Media id</param>
        /// <returns>Class representing media containing its information.</returns>
        public BaseMedia GetMediaById(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            // First, fetch the main media info
            const string mediaSql = @"
                SELECT id, title, description, media_type, release_year, age_restriction, genres, creator_id
                FROM media
                WHERE id = @id;
                ";

            BaseMedia media;

            using (var cmd = new NpgsqlCommand(mediaSql, conn))
            {
                cmd.Parameters.AddWithValue("id", id);

                using var reader = cmd.ExecuteReader();
                if (!reader.Read()) return null;

                media = new BaseMedia
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    MediaType = reader.GetString(3),
                    ReleaseYear = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                    AgeRestriction = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                    Genres = reader.IsDBNull(6) ? new List<string>() : reader.GetFieldValue<string[]>(6).ToList(),
                    UserId = reader.GetInt32(7),
                    Ratings = new List<Rating>()
                };
            }

            // Fetch ratings for the media
            const string ratingsSql = @"
                SELECT 
                r.id, r.user_id, u.username, r.score, r.comment, r.approved
                FROM ratings r
                JOIN users u ON r.user_id = u.id
                WHERE r.media_id = @mediaId;
                ";

            using (var cmd = new NpgsqlCommand(ratingsSql, conn))
            {
                cmd.Parameters.AddWithValue("mediaId", id);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var rating = new Rating
                    {
                        Id = reader.GetInt32(0),
                        User = new User
                        {
                            Id = reader.GetInt32(1),
                            Username = reader.GetString(2)
                        },
                        Score = reader.GetInt32(3),
                        Comment = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Approved = reader.GetBoolean(5),
                        Likes = new List<BaseUser>()
                    };

                    if (!rating.Approved && !String.IsNullOrEmpty(rating.Comment))
                       rating.Comment = "[AWAITING APPROVAL]"; // Hide comment if not approved

                    media.Ratings.Add(rating);
                }
            }
            
            media.RatingCount = media.Ratings.Count;
            media.AverageRating = media.RatingCount > 0 ? media.Ratings.Average(r => r.Score) : 0.0;

            // Fetch likes for each rating
            if (media.Ratings.Any())
            {
                const string likesCountSql = @"
                    SELECT rating_id, COUNT(*) AS like_count
                    FROM rating_likes
                    WHERE rating_id = ANY(@ratingIds)
                    GROUP BY rating_id;
                    ";

                using var cmd = new NpgsqlCommand(likesCountSql, conn);
                cmd.Parameters.AddWithValue("ratingIds", media.Ratings.Select(r => r.Id).ToArray());

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int ratingId = reader.GetInt32(0);
                    int likeCount = reader.GetInt32(1);

                    var rating = media.Ratings.FirstOrDefault(r => r.Id == ratingId);
                    if (rating != null)
                    {
                        rating.LikeCount = likeCount;
                    }
                }
            }

            // Fetch for favorites by users
            const string favoritesCountSql = @"
                SELECT COUNT(*) 
                FROM favorites
                WHERE media_id = @mediaId;
                ";

            using (var cmd = new NpgsqlCommand(favoritesCountSql, conn))
            {
                cmd.Parameters.AddWithValue("mediaId", id);
                media.FavoriteCount = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return media;
        }

        /// <summary>
        /// Deletes media.
        /// </summary>
        /// <param name="id">Media id to delete.</param>
        /// <returns>Boolean indicating whether the operation is successful.</returns>
        public bool DeleteMedia(int id)
        {
            if (id <= 0)
                return false;

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    DELETE FROM media
                    WHERE id = @id;
                    ";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("id", id);

                int affectedRows = cmd.ExecuteNonQuery();

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting media: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Updates media information,
        /// </summary>
        /// <param name="updatedMedia">Updated media information.</param>
        /// <param name="id">ID of media to update.</param>
        /// <returns>Boolean indicating whether the operation is successful.</returns>
        public bool UpdateMedia(BaseMedia updatedMedia, int id)
        {
            if (updatedMedia == null || string.IsNullOrWhiteSpace(updatedMedia.Title) || string.IsNullOrWhiteSpace(updatedMedia.MediaType))
                return false;

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    UPDATE media
                    SET
                    title = @title,
                    description = @description,
                    media_type = @mediaType,
                    release_year = @releaseYear,
                    age_restriction = @ageRestriction,
                    genres = @genres
                    WHERE id = @id;
                     ";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("title", updatedMedia.Title);
                cmd.Parameters.AddWithValue("description", (object)updatedMedia.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("mediaType", updatedMedia.MediaType);
                cmd.Parameters.AddWithValue("releaseYear", updatedMedia.ReleaseYear.HasValue ? (object)updatedMedia.ReleaseYear.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("ageRestriction", updatedMedia.AgeRestriction.HasValue ? (object)updatedMedia.AgeRestriction.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("genres", updatedMedia.Genres != null ? updatedMedia.Genres.ToArray() : new string[] { });
                cmd.Parameters.AddWithValue("creatorId", updatedMedia.UserId);

                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating media: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets a list of all media sorted by average rating..
        /// </summary>
        /// <returns>List of sorted media entries.</returns>
        public List<MediaSummaryDTO> GetLeaderboard()
        {
            var leaderboard = new List<MediaSummaryDTO>();

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                // Aggregate average rating for each media
                const string sql = @"
                    SELECT m.id, m.title, COALESCE(m.description, '') AS description, COALESCE(AVG(r.score), 0) AS average_rating
                    FROM media m
                    LEFT JOIN ratings r ON r.media_id = m.id
                    GROUP BY m.id, m.title, m.description
                    ORDER BY average_rating DESC, m.title ASC;
                    ";

                using var cmd = new NpgsqlCommand(sql, conn);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    leaderboard.Add(new MediaSummaryDTO
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Description = reader.GetString(2),
                        AverageRating = reader.GetInt32(3)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching leaderboard: {ex.Message}");                
            }

            return leaderboard;
        }

        /// <summary>
        /// Searches media based on different filters.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="genres"></param>
        /// <param name="mediaType"></param>
        /// <param name="releaseYear"></param>
        /// <param name="ageRestriction"></param>
        /// <param name="minRating"></param>
        /// <param name="sortBy"></param>
        /// <param name="ascending"></param>
        /// <returns>List of media results.</returns>
        public List<MediaSummaryDTO> SearchMedia(string? title = null,
                                                 string[]? genres = null,
                                                 string? mediaType = null,
                                                 int? releaseYear = null,
                                                 int? ageRestriction = null,
                                                 double? minRating = null,
                                                 string sortBy = "title", // "title", "year", "score"
                                                 bool ascending = true)
        {
            var results = new List<MediaSummaryDTO>();

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                // Base SQL which we will expand based on filters
                var sql = @"
                    SELECT m.id, m.title, COALESCE(m.description, '') AS description, COALESCE(AVG(r.score), 0.0) AS average_rating 
                    FROM media m
                    LEFT JOIN ratings r ON r.media_id = m.id
                    WHERE 1=1
                    ";

                // Different parameters to add based on filters
                var parameters = new List<NpgsqlParameter>();

                if (!string.IsNullOrWhiteSpace(title))
                {
                    sql += " AND m.title ILIKE @title"; // ILIKE for case-insensitive search: https://www.datacamp.com/doc/postgresql/ilike
                    parameters.Add(new NpgsqlParameter("title", $"%{title}%"));
                }

                if (!string.IsNullOrEmpty(mediaType))
                {
                    sql += " AND m.media_type = @mediaType";
                    parameters.Add(new NpgsqlParameter("mediaType", mediaType));
                }

                if (releaseYear.HasValue)
                {
                    sql += " AND m.release_year = @releaseYear";
                    parameters.Add(new NpgsqlParameter("releaseYear", releaseYear.Value));
                }

                if (ageRestriction.HasValue)
                {
                    sql += " AND m.age_restriction <= @ageRestriction";
                    parameters.Add(new NpgsqlParameter("ageRestriction", ageRestriction.Value));
                }

                if (genres != null && genres.Length > 0)
                {
                    sql += " AND m.genres && @genres"; // && is PostgreSQL array overlap operator
                    parameters.Add(new NpgsqlParameter("genres", genres));
                }

                // Group by needed because of AVG aggregation
                sql += " GROUP BY m.id, m.title, m.description";

                if (minRating.HasValue)
                {
                    sql += " HAVING COALESCE(AVG(r.score), 0) >= @minRating";
                    parameters.Add(new NpgsqlParameter("minRating", minRating.Value));
                }

                // Sorting
                string sortColumn = sortBy.ToLower() switch
                {
                    "year" => "m.release_year",
                    "score" => "average_rating",
                    _ => "m.title" 
                };

                sql += $" ORDER BY {sortColumn} {(ascending ? "ASC" : "DESC")}";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddRange(parameters.ToArray());

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(new MediaSummaryDTO
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Description = reader.GetString(2),
                        AverageRating = reader.GetDouble(3)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching media: {ex.Message}");
            }

            return results;
        }

    }
}
