namespace MediaRatingProject.API.Controllers
{
    using MediaRatingProject.API.DTOs;
    using MediaRatingProject.API.Interfaces;
    using MediaRatingProject.API.Requests;
    using MediaRatingProject.Data.Ratings;
    using MediaRatingProject.Data.Stores;
    using MediaRatingProject.Data.Users;
    using Npgsql;
    using System.Text.Json;

    public class UsersController
    {
        private UserStore _userStore;
        public readonly ITokenService _tokenService;

        public UsersController(UserStore store, FavoriteStore ratingStore, ITokenService jwtService)
        {
            _userStore = store;
            _tokenService = jwtService;
        }

        /// <summary>
        /// Registers a user in the system if there are no duplicate.
        /// </summary>
        /// <param name="request">The registeration request.</param>
        /// <returns>A HTTP response.</returns>
        public ResponseHandler Register(ParsedRequestDTO request)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                UserDTO userDto = JsonSerializer.Deserialize<UserDTO>(request.Body, options);

                if (userDto == null || string.IsNullOrWhiteSpace(userDto.Username) || string.IsNullOrWhiteSpace(userDto.Password))
                    return ResponseHandler.BadRequest("Invalid username or password.");

                // Check if user already exists
                //if (_userStore.GetUserByUsername(userDto.Username) != null)
                //    return ResponseHandler.BadRequest("User with the same name already exists!");

                // Create new user
                var newUser = new User(userDto.Username, userDto.Password);
                bool success = _userStore.CreateUser(newUser);
                if (!success)
                    return ResponseHandler.BadRequest("Failed to register user.");

                return ResponseHandler.Ok("User registered successfully.");
            }
            catch (JsonException ex)
            {
                return ResponseHandler.BadRequest($"Invalid JSON format: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest($"Error registering user: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs in a user and returns a JWT token if successful.
        /// </summary>
        /// <param name="request">The log in request.</param>
        /// <returns>A HTTP response.</returns>
        public ResponseHandler Login(ParsedRequestDTO request)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                UserDTO userDto = JsonSerializer.Deserialize<UserDTO>(request.Body, options);

                if (userDto == null || string.IsNullOrWhiteSpace(userDto.Username) || string.IsNullOrWhiteSpace(userDto.Password))
                    return ResponseHandler.BadRequest("Invalid username or password.");

                var existingUser = _userStore.GetUserByUsername(userDto.Username);
                if (existingUser == null || existingUser.Password != userDto.Password)
                    return ResponseHandler.Unauthorized("Invalid username or password.");

                // Generate JWT token to return.
                var token = _tokenService.GenerateToken(existingUser.Username);

                return ResponseHandler.Ok("Login successful.", new { token });
            }
            catch (JsonException ex)
            {
                return ResponseHandler.BadRequest($"Invalid JSON format: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest($"Error logging in: {ex.Message}");
            }
        }

        public ResponseHandler GetUserByID(ParsedRequestDTO request)
        {
            try
            {
                // Validate and parse user ID from parameters
                if (!request.Parameters.TryGetValue("userId", out var userIdStr) || !int.TryParse(userIdStr, out int userId))
                    return ResponseHandler.BadRequest("Missing or invalid user ID.");

                var user = _userStore.GetUserById(userId);
                if (user == null)
                    return ResponseHandler.NotFound("User not found.");

                var responseBody = new
                {
                    user.Id,
                    user.Username,
                    user.RatedMediaCount,
                    user.FavoriteMediaCount
                };

                return ResponseHandler.Ok("User fetched successfully.", responseBody);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest($"Error fetching user: {ex.Message}");
            }
        }

        public int? GetUserIdByUsername(ParsedRequestDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.UserName))
                    return null;
                var user = _userStore.GetUserByUsername(request.UserName);
                return user?.Id;
            }
            catch
            {
                return null;
            }
        }
    }
}
