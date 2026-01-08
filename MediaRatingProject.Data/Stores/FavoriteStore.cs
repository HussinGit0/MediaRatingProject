namespace MediaRatingProject.Data.Stores
{
    using MediaRatingProject.Data.Media;
    using MediaRatingProject.Data.Ratings;
    using MediaRatingProject.Data.StoreInterfaces;
    using Npgsql;

    /// <summary>
    /// Class responsible for managing favorites in the database.
    /// </summary>
    public class FavoriteStore: IFavoriteStore
    {
        private readonly string _connectionString;

        public FavoriteStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Gets the favorites of a user by its id.
        /// </summary>
        /// <param name="userId">The requester's ID.</param>
        /// <returns>A list of media which is favorited by the requester.</returns>
        public List<BaseMedia> GetFavoritesByUserID(int? userId)
        {
            var favorites = new List<BaseMedia>();
            if (userId <= 0) return favorites;

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                const string sql = @"
                SELECT m.id, m.title, m.description, m.media_type, m.release_year, m.age_restriction, m.genres, m.creator_id
                FROM media m
                JOIN favorites f ON m.id = f.media_id
                WHERE f.user_id = @userId;
            ";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("userId", userId);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var media = new BaseMedia
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

                    favorites.Add(media);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching favorites: {ex.Message}");
            }

            return favorites;
        }

        /// <summary>
        /// Marks a media as favorite for a user.
        /// </summary>
        /// <param name="userId">The requester's ID</param>
        /// <param name="mediaId">The Media's ID</param>
        /// <returns>Boolean indicating whether the operation is successful.</returns>
        public bool MarkFavorite(int? userId, int mediaId)
        {
            if (userId == null || mediaId == null) return false;
            if (userId <= 0 || mediaId <= 0) return false;

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    INSERT INTO favorites (user_id, media_id)
                    VALUES (@userId, @mediaId)
                    ON CONFLICT DO NOTHING;
                    ";

                using var cmd = new NpgsqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("userId", userId);
                cmd.Parameters.AddWithValue("mediaId", mediaId);

                int affectedRows = cmd.ExecuteNonQuery();
                return affectedRows > 0; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking favorite: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Unmarks a media as favorite for a user.
        /// </summary>
        /// <param name="userId">The requester's ID</param>
        /// <param name="mediaId">The Media's ID</param>
        /// <returns>Boolean indicating whether the operation is successful.</returns>
        public bool UnmarkFavorite(int? userId, int mediaId)
        {
            if (userId == null || mediaId == null) return false;
            if (userId <= 0 || mediaId <= 0) return false;

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                const string sql = @"
                    DELETE FROM favorites
                    WHERE user_id = @userId AND media_id = @mediaId;
                    ";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("userId", userId);
                cmd.Parameters.AddWithValue("mediaId", mediaId);

                int affectedRows = cmd.ExecuteNonQuery();
                return affectedRows > 0; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error unmarking favorite: {ex.Message}");
                return false;
            }
        }

    }
}
