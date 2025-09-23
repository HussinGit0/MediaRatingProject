namespace API
{
    using System.Net;
    using System.IO;
    using System.Text.Json;
    using MediaRatingProject.Server.Requests;
    using MediaRatingProject.DB.Users;

    /// <summary>
    /// A basic listener class that listens to HTTP requests and handles them.
    /// Contains only testing functionality at the moment.
    /// Based on https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistener?view=net-9.0
    /// </summary>
    internal class APIListener
    {
        private readonly HttpListener _listener;
        private UserStore UserDB;

        /// <summary>
        /// A listener 
        /// </summary>
        /// <param name="prefixes"></param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public APIListener(string[] prefixes)
        {
            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("HttpListener is not supported on this platform.");
            }

            if (prefixes == null || prefixes.Length == 0)
            {
                throw new ArgumentException("At least one prefix is required.");
            }

            _listener = new HttpListener();
            foreach (string prefix in prefixes)
            {
                _listener.Prefixes.Add(prefix);
            }

            /// TESTING PURPOSES ONLY
            this.UserDB = new UserStore();
            this.UserDB.Users.Add(new User("alice", "1234"));
            ///////////////////////////////////////////
        }

        public void Start()
        {
            _listener.Start();

            const string TESTREQUEST = "/api/test";
            const string REGISTERREQUEST = "/api/register";
            const string LOGINREQUEST = "/api/login";

            string body;

            while (true)
            {
                Console.WriteLine("Listening...");

                HttpListenerContext context = _listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                Console.WriteLine($"[{request.HttpMethod}] {request.Url}");

                if (request.HttpMethod == "GET" && request.Url.AbsolutePath == TESTREQUEST)
                {
                    body = GetBody(request);

                    Console.WriteLine($"Received body: {body}");

                    HandleGETRequest(body);
                }

                if (request.HttpMethod == "POST")
                {
                    body = GetBody(request);
                    if (request.Url.AbsolutePath == LOGINREQUEST)
                        LoginUser(body);

                    if (request.Url.AbsolutePath == REGISTERREQUEST)
                        RegisterUser(body);
                }

                response.StatusCode = 200;
                response.Close();
            }
        }

        private void HandleGETRequest(string body)
        {
            if (body == "testing")
            {
                Console.WriteLine("Success!");
            }
            else if (body == "rnd")
            {
                Random rnd = new();
                Console.WriteLine($"Random number: {rnd.Next()}");
            }
            else
            {
                Console.WriteLine("Unknown request!");
            }
        }

        private string GetBody(HttpListenerRequest request)
        {
            string body;
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                body = reader.ReadToEnd();
            }

            return body;
        }

        /// <summary>
        /// FOR TESTING PURPOSES 
        /// </summary>
        /// <param name="body"></param>
        private void RegisterUser(string body)
        {
            var loginRequest = JsonSerializer.Deserialize<LoginRequest>(body)!;

            this.UserDB.Users.Add(new User(loginRequest.Username, loginRequest.Password));
            Console.WriteLine($"Sucessfully registered {loginRequest.Username}.");
        }

        /// <summary>
        /// FOR TESTING PURPOSES 
        /// </summary>
        /// <param name="body"></param>
        private void LoginUser(string body)
        {
            var loginRequest = JsonSerializer.Deserialize<LoginRequest>(body)!;
        }
    }
}
