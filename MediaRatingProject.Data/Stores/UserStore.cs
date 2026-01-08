namespace MediaRatingProject.Data.Stores
{
    using MediaRatingProject.Data.Ratings;
    using MediaRatingProject.Data.StoreInterfaces;
    using MediaRatingProject.Data.Users;
    using Npgsql;

    public class UserStore: IUserStore
    {
        private readonly string _connectionString;

        public UserStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Creates a new user in the database.
        /// </summary>
        /// <param name="user">User information</param>
        /// <returns>Boolean indicating whether the operation is successful.</returns>
        public bool CreateUser(BaseUser user)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                if (GetUserByUsername(user.Username) != null)
                {
                    Console.WriteLine($"User with the same username {user.Username} already exists.");
                    return false;
                }

                const string sql = @"
                    INSERT INTO users (username, password)
                    VALUES (@username, @password)
                    RETURNING id;
                    ";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("username", user.Username);
                    cmd.Parameters.AddWithValue("password", user.Password);
                    user.Id = (int)cmd.ExecuteScalar();

                    Console.WriteLine($"User '{user.Username}' created with ID {user.Id}");
                }
            }

            return true;
        }

        /// <summary>
        /// This function is not part of the specifications but is useful for testing. In a real system, user removal
        /// would be handled through an admin interface with proper authentication and authorization.
        /// </summary>
        /// <param name="userId">The ID of the user to delete</param>
        /// <returns>True if the deletion was successful.</returns>
        public bool RemoveUser(int userId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            const string sql = @"
                DELETE FROM users
                WHERE id = @id;
                ";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", userId);

            int affectedRows = cmd.ExecuteNonQuery();
            return affectedRows > 0; // returns true if a user was deleted
        }

        /// <summary>
        /// This function is not part of the specifications but is useful for testing.
        /// </summary>
        /// <param name="updatedUser">New user information.</param>
        /// <param name="id">User id to update.</param>
        /// <returns>True if the user is updated.</returns>
        public bool UpdateUser(BaseUser updatedUser, int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            const string sql = @"
                UPDATE users
                SET username = @username,
                password = @password
                WHERE id = @id;
                ";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("username", updatedUser.Username);
            cmd.Parameters.AddWithValue("password", updatedUser.Password);
            cmd.Parameters.AddWithValue("id", id);

            int affectedRows = cmd.ExecuteNonQuery();
            return affectedRows > 0; // returns true if a user was updated
        }

        /// <summary>
        /// Gets a user by their ID.
        /// </summary>
        /// <param name="id">The user's id.</param>
        /// <returns>A user class containing the user's information.</returns>
        public BaseUser GetUserById(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            const string sql = @"
                SELECT 
                u.id,
                u.username,
                COUNT(DISTINCT r.id) AS rated_count,
                COUNT(DISTINCT f.media_id) AS favorite_count
                FROM users u
                LEFT JOIN ratings r ON r.user_id = u.id
                LEFT JOIN favorites f ON f.user_id = u.id
                WHERE u.id = @id
                GROUP BY u.id, u.username;
                ";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                RatedMediaCount = reader.GetInt32(2),
                FavoriteMediaCount = reader.GetInt32(3)
            };
        }

        /// <summary>
        /// Gets a user by its Username.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public BaseUser GetUserByUsername(string username)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                const string sql = @"
                    SELECT id, username, password
                    FROM users
                    WHERE username = @username
                    ";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("username", username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;

                        return new User
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Password = reader.GetString(2)
                        };
                    }
                }
            }
        }
    }
}
