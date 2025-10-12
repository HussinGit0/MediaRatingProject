
namespace API
{
    using MediaRatingProject.DB.Users;
    using System.Text.Json;
    using API.Requests;

    public class UsersHandler
    {
        private readonly UserStore _userStore;

        public UsersHandler(UserStore userStore)
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
        }
    }
}
