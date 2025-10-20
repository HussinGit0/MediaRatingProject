namespace MediaRatingProject.API
{
    using System.Net;

    public class RequestParser
    {
        private UsersHandler _usersHandler;

        public RequestParser(UsersHandler usersHandler)
        {
            _usersHandler = usersHandler;
        }

        public void HandleRequest(HttpListenerRequest request, string body)
        {
            //HttpListenerRequest request = listner.GetContext().Request;

            switch (request.HttpMethod.ToUpper())
            {
                case "GET":
                    HandleGETRequest(request, body);
                    break;
                case "POST":
                    HandlePOSTRequest(request, body);
                    break;
                case "PUT":
                    HandlePUTRequest(request, body);
                    break;
                case "DELETE":
                    HandleDELETERequest(request, body);
                    break;
                default:
                    Console.WriteLine($"Unsupported HTTP method: {request.HttpMethod}");
                    break;
            }
        }

        private void HandlePOSTRequest(HttpListenerRequest request, string body)
        {
            Dictionary<string, string> parameters;

            if (request.Url.AbsolutePath == EndPoints.USERS_LOGIN_REQUEST)
                Console.WriteLine("POST: Login requested!");

            else if (RouteMatcher.TryMatch(EndPoints.USERS_PROFILE_REQUEST, request.Url.AbsolutePath, out parameters))
            {
                Console.WriteLine($"POST: Profile number {int.Parse(parameters["id"])} requested!");
            }

            else if (request.Url.AbsolutePath == EndPoints.USERS_REGISTER_REQUEST)
                Console.WriteLine("POST: Registration requested!");
            else if (request.Url.AbsolutePath == EndPoints.MEDIA_REQUEST)
                Console.WriteLine("POST: Media requested!");


            else if (RouteMatcher.TryMatch(EndPoints.MEDIA_RATE_REQUEST, request.Url.AbsolutePath, out parameters))
                Console.WriteLine($"POST: Rate media ID {parameters["mediaId"]}");
            else if (RouteMatcher.TryMatch(EndPoints.MEDIA_FAVORITE_REQUEST, request.Url.AbsolutePath, out parameters))
                Console.WriteLine($"POST: Mark media ID {parameters["mediaId"]} as favorite");
            else if (RouteMatcher.TryMatch(EndPoints.MEDIA_LIKE_REQUEST, request.Url.AbsolutePath, out parameters))
                Console.WriteLine($"POST: Like rating ID {parameters["ratingId"]}");
            else if (RouteMatcher.TryMatch(EndPoints.RATINGS_ID_CONFIRM_REQUEST, request.Url.AbsolutePath, out parameters))
                Console.WriteLine($"POST: Confirm rating ID {parameters["ratingId"]}");

            // Default fallback
            else
                Console.WriteLine($"=============\nPOST request to unknown endpoint: {request.Url.AbsolutePath}\n=============");
        }

        private void HandleGETRequest(HttpListenerRequest request, string body)
        {
            Dictionary<string, string> parameters;

            // User endpoints
            if (RouteMatcher.TryMatch(EndPoints.USERS_PROFILE_REQUEST, request.Url.AbsolutePath, out parameters))
                Console.WriteLine($"GET: Fetch profile for user ID {parameters["userId"]}");
            else if (RouteMatcher.TryMatch(EndPoints.USERS_RATINGS_REQUEST, request.Url.AbsolutePath, out parameters))
                Console.WriteLine($"GET: Fetch ratings for user ID {parameters["userId"]}");
            else if (RouteMatcher.TryMatch(EndPoints.USERS_FAVORITES_REQUEST, request.Url.AbsolutePath, out parameters))
                Console.WriteLine($"GET: Fetch favorites for user ID {parameters["userId"]}");
            else if (RouteMatcher.TryMatch(EndPoints.USERS_RECOMMENDATION_REQUEST, request.Url.AbsolutePath, out parameters))
            {
                var queryType = request.QueryString["type"] ?? "default";
                Console.WriteLine($"GET: Recommendations for user ID {parameters["userId"]}, type {queryType}");
            }
            // Media endpoints
            else if (RouteMatcher.TryMatch(EndPoints.MEDIA_ID_REQUEST, request.Url.AbsolutePath, out parameters))
                Console.WriteLine($"GET: Fetch media with ID {parameters["mediaId"]}");            
            else if (request.Url.AbsolutePath == EndPoints.MEDIA_REQUEST)
                Console.WriteLine("GET: List media entries");            
            // Leaderboard
            else if (request.Url.AbsolutePath == EndPoints.LEADERBOARD_REQUEST)
                Console.WriteLine("GET: Fetch leaderboard");
            // Default fallback
            else
                Console.WriteLine($"=============\nGET request to unknown endpoint: {request.Url.AbsolutePath}\n=============");
        }

        private void HandlePUTRequest(HttpListenerRequest request, string body)
        {
            Dictionary<string, string> parameters;

            // User profile update
            if (RouteMatcher.TryMatch(EndPoints.USERS_PROFILE_REQUEST, request.Url.AbsolutePath, out parameters))
                Console.WriteLine($"PUT: Update profile for user ID {parameters["userId"]}");

            // Media update
            else if (RouteMatcher.TryMatch(EndPoints.MEDIA_ID_REQUEST, request.Url.AbsolutePath, out parameters))
                Console.WriteLine($"PUT: Update media ID {parameters["mediaId"]}");

            // Rating update
            else if (RouteMatcher.TryMatch(EndPoints.RATINGS_ID_REQUEST, request.Url.AbsolutePath, out parameters))
                Console.WriteLine($"PUT: Update rating ID {parameters["ratingId"]}");

            else
                Console.WriteLine($"=============\nPUT request to unknown endpoint: {request.Url.AbsolutePath}\n=============");
        }

        private void HandleDELETERequest(HttpListenerRequest request, string body)
        {
            Dictionary<string, string> parameters;

            // Media delete
            if (RouteMatcher.TryMatch(EndPoints.MEDIA_ID_REQUEST, request.Url.AbsolutePath, out parameters))
                Console.WriteLine($"DELETE: Delete media ID {parameters["mediaId"]}");

            // Media un-favorite
            else if (RouteMatcher.TryMatch(EndPoints.MEDIA_FAVORITE_REQUEST, request.Url.AbsolutePath, out parameters))
                Console.WriteLine($"DELETE: Unmark media ID {parameters["mediaId"]} as favorite");

            // Rating delete
            else if (RouteMatcher.TryMatch(EndPoints.RATINGS_ID_REQUEST, request.Url.AbsolutePath, out parameters))
                Console.WriteLine($"DELETE: Delete rating ID {parameters["ratingId"]}");

            else
                Console.WriteLine($"=============\nDELETE request to unknown endpoint: {request.Url.AbsolutePath}\n=============");
        }
    }
}
