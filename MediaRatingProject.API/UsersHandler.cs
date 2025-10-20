namespace MediaRatingProject.API
{
    using MediaRatingProject.API.Interfaces;
    using MediaRatingProject.API.Requests;
    using MediaRatingProject.API.Services;
    //using MediaRatingProject.DB.Users;
    using System.Text.Json;

    public class UsersHandler
    {
        //private readonly UserStore _userStore;

        private readonly ITokenService _jwtService;

        public UsersHandler(ITokenService jwtService)
        {
            _jwtService = jwtService;
        }

        public string Login(string username, string password)
        {
            // TODO: Validate user credentials from DB
            if (username == "mustermann" && password == "max")
            {
                return _jwtService.GenerateToken(username);
            }

            return null;
        }
        /*public UsersHandler(UserStore userStore)
        {
            _userStore = userStore;

            /// TESTING PURPOSES ONLY
            
            _userStore.Users.Add(new User("alice", "1234"));
            ///////////////////////////////////////////
        }

        /// <summary>
        /// FOR TESTING PURPOSES 
        /// </summary>
        /// <param name="body"></param>
        public void RegisterUser(string body)
        {
            var loginRequest = JsonSerializer.Deserialize<LoginRequest>(body)!;

            _userStore.Users.Add(new User(loginRequest.Username, loginRequest.Password));
            Console.WriteLine($"Sucessfully registered {loginRequest.Username}.");
        }

        /// <summary>
        /// FOR TESTING PURPOSES 
        /// </summary>
        /// <param name="body"></param>
        public void LoginUser(string body)
        {
            var loginRequest = JsonSerializer.Deserialize<LoginRequest>(body)!;
        }*/
    }
}
