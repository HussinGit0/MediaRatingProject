namespace API
{
    using System.Net;

    public class RequestHandler
    {
        const string TESTREQUEST = "/api/test";
        const string REGISTERREQUEST = "/api/register";
        const string LOGINREQUEST = "/api/login";

        private UsersHandler _usersHandler;

        public RequestHandler(UsersHandler usersHandler)
        {
            _usersHandler = usersHandler;
        }

        public void HandleRequest(HttpListenerRequest request, string body)
        {
            switch (request.HttpMethod)
            {
                case "GET":
                    HandleGETRequest(request, body);
                    break;
                case "POST":
                    HandlePOSTRequest(request, body);
                    break;
                default:
                    Console.WriteLine($"Unsupported HTTP method: {request.HttpMethod}");
                    break;
            }
        }

        private void HandlePOSTRequest(HttpListenerRequest request, string body)
        {
            if (request.Url.AbsolutePath == LOGINREQUEST)
                _usersHandler.LoginUser(body);

            if (request.Url.AbsolutePath == REGISTERREQUEST)
                _usersHandler.RegisterUser(body);
        }

        private void HandleGETRequest(HttpListenerRequest request, string body)
        {
            if (request.Url.AbsolutePath == TESTREQUEST)
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
            else
            {
                Console.WriteLine($"Unknown GET endpoint: {request.Url.AbsolutePath}");
            }
        }       
    }
}
