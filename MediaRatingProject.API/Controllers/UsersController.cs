namespace MediaRatingProject.API.Controllers
{
    using MediaRatingProject.API.Interfaces;
    using MediaRatingProject.API.Requests;
    using MediaRatingProject.Data.Stores;
    using MediaRatingProject.Data.Users;
    using System.Text.Json;

    public class UsersController
    {
        private UserStore _userStore;
        private readonly ITokenService _jwtService;

        public UsersController(UserStore store, ITokenService jwtService)
        {
            _userStore = store;
            _jwtService = jwtService;
        }

        public ResponseHandler Register(ParsedRequestDTO request)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                UserDTO userDto = JsonSerializer.Deserialize<UserDTO>(request.Body, options);
                
                if (userDto == null || string.IsNullOrWhiteSpace(userDto.Username) || string.IsNullOrWhiteSpace(userDto.Password))
                    return ResponseHandler.BadRequest("Invalid username or password.");

                // Check if user already exists
                if (_userStore.GetUserByUsername(userDto.Username) != null)
                    return ResponseHandler.BadRequest("User with the same name already exists!");

                // Create new user
                var newUser = new User(userDto.Username, userDto.Password);
                bool success = _userStore.AddUser(newUser);
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

                // Generate JWT token
                var token = _jwtService.GenerateToken(existingUser.Username);

                return ResponseHandler.Ok("Login successful.", JsonSerializer.Serialize(new { token }));
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

        public class UserDTO
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
