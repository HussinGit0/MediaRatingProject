namespace MediaRatingProject.Data.Stores
{
    using MediaRatingProject.Data.Media;
    using MediaRatingProject.Data.Ratings;
    using MediaRatingProject.Data.StoreInterfaces;
    using MediaRatingProject.Data.Users;
    using Npgsql;

    public class RatingStore: IRatingStore
    {
        private readonly string _connectionString;

        public RatingStore(string connectionString)
        {
            _connectionString = connectionString;
        }
        /// <summary>
        /// Gets all ratings created by a user.
        /// </summary>
        /// <param name="userId">Users ID</param>
        /// <returns>A list of ratings for the user's information.</returns>
        public List<Rating> GetRatingsByUser(int userId)
        {
            var ratings = new List<Rating>();
            if (userId <= 0) return ratings;

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    SELECT r.id, r.score, r.comment, r.approved, m.id AS media_id, m.title, m.description, COUNT(rl.user_id) AS like_count
                    FROM ratings r
                    JOIN media m ON m.id = r.media_id
                    LEFT JOIN rating_likes rl ON rl.rating_id = r.id
                    WHERE r.user_id = @userId
                    GROUP BY r.id, m.id, m.title;
                    ";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("userId", userId);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var rating = new Rating
                    {
                        Id = reader.GetInt32(0),
                        Score = reader.GetInt32(1),
                        Comment = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Approved = reader.GetBoolean(3),

                        Media = new MediaSummaryDTO
                        {
                            Id = reader.GetInt32(4),
                            Title = reader.GetString(5),
                            Description = reader.IsDBNull(6) ? "" : reader.GetString(6)
                        },

                        LikeCount = reader.GetInt32(7),
                        Likes = new List<BaseUser>()
                    };

                    if (!rating.Approved && !String.IsNullOrEmpty(rating.Comment))
                        rating.Comment = "[AWAITING APPROVAL]"; // Hide comment if not approved

                    rating.LikeCount = rating.Likes.Count > 0 ? rating.Likes.Count : 0;

                    ratings.Add(rating);                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching ratings: {ex.Message}");
            }

            return ratings;
        }

        /// <summary>
        /// Creates a new rating in the database.
        /// </summary>
        /// <param name="rating">Rating info.</param>
        /// <returns>Boolean indicating whether the operation is successful.</returns>
        public bool CreateRating(Rating rating)
        {
            if (rating == null ||
                rating.User == null ||
                rating.Media == null ||
                rating.User.Id <= 0 ||
                rating.Media.Id <= 0 ||
                rating.Score < 1 || rating.Score > 5)
            {
                return false;
            }

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    INSERT INTO ratings (user_id, media_id, score, comment, approved)
                    VALUES (@userId, @mediaId, @score, @comment, @approved);
                    ";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("userId", rating.User.Id);
                cmd.Parameters.AddWithValue("mediaId", rating.Media.Id);
                cmd.Parameters.AddWithValue("score", rating.Score);
                cmd.Parameters.AddWithValue("comment", (object?)rating.Comment ?? DBNull.Value);
                cmd.Parameters.AddWithValue("approved", rating.Approved);

                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating rating: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Updates the rating of a user.
        /// </summary>
        /// <param name="updatedRating">The new rating information.</param>
        /// <returns>Boolean indicating whether the operation is successful.</returns>
        public bool UpdateRating(Rating updatedRating)
        {
            if (updatedRating == null ||
                updatedRating.Id <= 0 ||
                updatedRating.User == null ||
                updatedRating.User.Id <= 0 ||
                updatedRating.Score < 1 || updatedRating.Score > 5)
            {
                return false; // invalid rating
            }

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    UPDATE ratings
                    SET score = @score,
                    comment = @comment,
                    approved = @approved
                    WHERE id = @ratingId AND user_id = @userId;
                    ";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("score", updatedRating.Score);
                cmd.Parameters.AddWithValue("comment", (object?)updatedRating.Comment ?? DBNull.Value);
                cmd.Parameters.AddWithValue("approved", updatedRating.Approved);
                cmd.Parameters.AddWithValue("ratingId", updatedRating.Id);
                cmd.Parameters.AddWithValue("userId", updatedRating.User.Id);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0; // true if a row was updated
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating rating: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Likes a rating for a given user.
        /// </summary>
        /// <param name="ratingId">The rating to like.</param>
        /// <param name="userId">The liker's ID</param>
        /// <returns>Boolean indicating whether the operation is successful.</returns>
        public bool LikeRating(int ratingId, int userId)
        {
            if (ratingId <= 0 || userId <= 0)
                return false;

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    INSERT INTO rating_likes (rating_id, user_id)
                    VALUES (@ratingId, @userId)
                    ON CONFLICT (rating_id, user_id) DO NOTHING;
                    ";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("ratingId", ratingId);
                cmd.Parameters.AddWithValue("userId", userId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error liking rating: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Approves comment of a media rating if the approver is the creator of the media.
        /// </summary>
        /// <param name="ratingId">Rating ID</param>
        /// <param name="approverId">Media creator ID.</param>
        /// <returns>Boolean indicating whether the operation is successful.</returns>
        public bool ApproveRating(int ratingId, int approverId)
        {
            if (ratingId <= 0 || approverId <= 0)
                return false;

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                // Only approve if the approver is the creator of the media
                const string sql = @"
                    UPDATE ratings r
                    SET approved = TRUE
                    FROM media m
                    WHERE r.id = @ratingId
                    AND r.media_id = m.id
                    AND m.creator_id = @approverId
                    AND r.approved = FALSE;
                    ";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("ratingId", ratingId);
                cmd.Parameters.AddWithValue("approverId", approverId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error approving rating: {ex.Message}");
                return false;
            }
        }
    }
}
