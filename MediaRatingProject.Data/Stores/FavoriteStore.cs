using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingProject.Data.Stores
{    
    public class FavoriteStore
    {
        private readonly string _connectionString;

        public FavoriteStore(string connectionString)
        {
            _connectionString = connectionString;
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
