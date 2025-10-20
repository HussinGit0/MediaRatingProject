namespace MediaRatingProject.API
{
    using MediaRatingProject.API.Controllers;
    using MediaRatingProject.API.Interfaces;
    using MediaRatingProject.API.Requests;
    using MediaRatingProject.API.Services;
    using System.Net;

    public class RequestParser
    {
        private UsersController _usersController;

        public RequestParser(UsersController usersController)
        {
            _usersController = usersController;
        }

        public ParsedRequestDTO ParseRequest(HttpListenerRequest request, string body)
        {
            //HttpListenerRequest request = listener.GetContext().Request;
            ParsedRequestDTO parsedRequestDTO;
            switch (request.HttpMethod.ToUpper())
            {
                case "GET":
                    parsedRequestDTO = ParseGETRequest(request, body);
                    break;
                case "POST":
                    parsedRequestDTO = ParsePOSTRequest(request, body);
                    break;
                case "PUT":
                    parsedRequestDTO = ParsePUTRequest(request, body);
                    break;
                case "DELETE":
                    parsedRequestDTO =  ParseDELETERequest(request, body);
                    break;
                default:
                    parsedRequestDTO = new() { IsSuccessful = false };
                    Console.WriteLine($"Unsupported HTTP method: {request.HttpMethod}");
                    break;
            }

            return parsedRequestDTO;
        }

        private ParsedRequestDTO ParsePOSTRequest(HttpListenerRequest request, string body)
        {
            ParsedRequestDTO requestDTO = new();
            requestDTO.Body = body;
            requestDTO.HttpMethod = "POST";
            requestDTO.IsSuccessful = true; // Assume success until failure.

            Dictionary<string, string> parameters = new();

            // This linear IF-ELSE is ineffecient, but serves its purpose for the intermediate version.
            // The plan for the final iteration is to create a more robust look-up table for additional effecincy.
            if (request.Url.AbsolutePath == EndPoints.USERS_LOGIN_REQUEST)
            {
                requestDTO.Path = EndPoints.USERS_LOGIN_REQUEST;
                Console.WriteLine("POST: Login requested!");
            }
            else if (request.Url.AbsolutePath == EndPoints.USERS_REGISTER_REQUEST)
            {
                requestDTO.Path = EndPoints.USERS_REGISTER_REQUEST;
                Console.WriteLine("POST: Registration requested!");
            }
            else if (request.Url.AbsolutePath == EndPoints.MEDIA_REQUEST)
            {
                requestDTO.Path = EndPoints.MEDIA_REQUEST;
                Console.WriteLine("POST: Media requested!");
            }
            else if (RouteMatcher.TryMatch(EndPoints.USERS_PROFILE_REQUEST, request.Url.AbsolutePath, out parameters))
            {
                requestDTO.Path = EndPoints.USERS_PROFILE_REQUEST;
                Console.WriteLine($"POST: Profile number {int.Parse(parameters["id"])} requested!");
            }
            else if (RouteMatcher.TryMatch(EndPoints.MEDIA_RATE_REQUEST, request.Url.AbsolutePath, out parameters))
            {
                requestDTO.Path = EndPoints.MEDIA_RATE_REQUEST;
                Console.WriteLine($"POST: Rate media ID {parameters["mediaId"]}");
            }
            else if (RouteMatcher.TryMatch(EndPoints.MEDIA_FAVORITE_REQUEST, request.Url.AbsolutePath, out parameters))
            {
                requestDTO.Path = EndPoints.MEDIA_FAVORITE_REQUEST;
                Console.WriteLine($"POST: Mark media ID {parameters["mediaId"]} as favorite");
            }
            else if (RouteMatcher.TryMatch(EndPoints.MEDIA_LIKE_REQUEST, request.Url.AbsolutePath, out parameters))
            {
                requestDTO.Path = EndPoints.MEDIA_LIKE_REQUEST;
                Console.WriteLine($"POST: Like rating ID {parameters["ratingId"]}");
            }
            else if (RouteMatcher.TryMatch(EndPoints.RATINGS_ID_CONFIRM_REQUEST, request.Url.AbsolutePath, out parameters))
            {
                requestDTO.Path = EndPoints.RATINGS_ID_CONFIRM_REQUEST;
                Console.WriteLine($"POST: Confirm rating ID {parameters["ratingId"]}");
            }
            else  // Default fallback
            {
                requestDTO.IsSuccessful = false;
                Console.WriteLine($"=============\nPOST request to unknown endpoint: {request.Url.AbsolutePath}\n=============");
            }

            requestDTO.Parameters = parameters;
            return requestDTO;
        }

        private ParsedRequestDTO ParseGETRequest(HttpListenerRequest request, string body)
        {
            ParsedRequestDTO requestDTO = new();
            requestDTO.Body = body;
            requestDTO.HttpMethod = "GET";
            requestDTO.IsSuccessful = true; // Assume success until failure.

            Dictionary<string, string> parameters = new();

            // User endpoints
            if (RouteMatcher.TryMatch(EndPoints.USERS_PROFILE_REQUEST, request.Url.AbsolutePath, out parameters))
            {
                requestDTO.Path = EndPoints.USERS_PROFILE_REQUEST;
                Console.WriteLine($"GET: Fetch profile for user ID {parameters["userId"]}");
            }
            else if (RouteMatcher.TryMatch(EndPoints.USERS_RATINGS_REQUEST, request.Url.AbsolutePath, out parameters))
            {
                requestDTO.Path = EndPoints.USERS_RATINGS_REQUEST;
                Console.WriteLine($"GET: Fetch ratings for user ID {parameters["userId"]}");
            }
            else if (RouteMatcher.TryMatch(EndPoints.USERS_FAVORITES_REQUEST, request.Url.AbsolutePath, out parameters))
            {
                requestDTO.Path = EndPoints.USERS_FAVORITES_REQUEST;
                Console.WriteLine($"GET: Fetch favorites for user ID {parameters["userId"]}");
            }
            else if (RouteMatcher.TryMatch(EndPoints.USERS_RECOMMENDATION_REQUEST, request.Url.AbsolutePath, out parameters))
            {
                requestDTO.Path = EndPoints.USERS_RECOMMENDATION_REQUEST;
                var queryType = request.QueryString["type"] ?? "default";
                Console.WriteLine($"GET: Recommendations for user ID {parameters["userId"]}, type {queryType}");
            }
            else if (RouteMatcher.TryMatch(EndPoints.MEDIA_ID_REQUEST, request.Url.AbsolutePath, out parameters))
            {
                requestDTO.Path = EndPoints.MEDIA_ID_REQUEST;
                Console.WriteLine($"GET: Fetch media with ID {parameters["mediaId"]}");
            }
            else if (request.Url.AbsolutePath == EndPoints.MEDIA_REQUEST)
            {
                requestDTO.Path = EndPoints.MEDIA_REQUEST;
                Console.WriteLine("GET: List media entries");
            }
            else if (request.Url.AbsolutePath == EndPoints.LEADERBOARD_REQUEST)
            {
                requestDTO.Path = EndPoints.LEADERBOARD_REQUEST;
                Console.WriteLine("GET: Fetch leaderboard");
            }
            else
            {
                requestDTO.IsSuccessful = false;
                Console.WriteLine($"=============\nGET request to unknown endpoint: {request.Url.AbsolutePath}\n=============");
            }

            requestDTO.Parameters = parameters;
            return requestDTO;
        }

        private ParsedRequestDTO ParsePUTRequest(HttpListenerRequest request, string body)
        {
            ParsedRequestDTO requestDTO = new();
            requestDTO.Body = body;
            requestDTO.HttpMethod = "PUT";
            requestDTO.IsSuccessful = true; // Assume success until failure.

            Dictionary<string, string> parameters = new();

            if (RouteMatcher.TryMatch(EndPoints.USERS_PROFILE_REQUEST, request.Url.AbsolutePath, out parameters))
            {
                requestDTO.Path = EndPoints.USERS_PROFILE_REQUEST;
                Console.WriteLine($"PUT: Update profile for user ID {parameters["userId"]}");
            }
            else if (RouteMatcher.TryMatch(EndPoints.MEDIA_ID_REQUEST, request.Url.AbsolutePath, out parameters))
            {
                requestDTO.Path = EndPoints.MEDIA_ID_REQUEST;
                Console.WriteLine($"PUT: Update media ID {parameters["mediaId"]}");
            }
            else if (RouteMatcher.TryMatch(EndPoints.RATINGS_ID_REQUEST, request.Url.AbsolutePath, out parameters))
            {
                requestDTO.Path = EndPoints.RATINGS_ID_REQUEST;
                Console.WriteLine($"PUT: Update rating ID {parameters["ratingId"]}");
            }
            else
            {
                requestDTO.IsSuccessful = false;
                Console.WriteLine($"=============\nPUT request to unknown endpoint: {request.Url.AbsolutePath}\n=============");
            }

            requestDTO.Parameters = parameters;
            return requestDTO;
        }

        private ParsedRequestDTO ParseDELETERequest(HttpListenerRequest request, string body)
        {
            ParsedRequestDTO requestDTO = new();
            requestDTO.Body = body;
            requestDTO.HttpMethod = "DELETE";
            requestDTO.IsSuccessful = true; // Assume success until failure.

            Dictionary<string, string> parameters = new();

            if (RouteMatcher.TryMatch(EndPoints.MEDIA_ID_REQUEST, request.Url.AbsolutePath, out parameters))
            {
                requestDTO.Path = EndPoints.MEDIA_ID_REQUEST;
                Console.WriteLine($"DELETE: Delete media ID {parameters["mediaId"]}");
            }
            else if (RouteMatcher.TryMatch(EndPoints.MEDIA_FAVORITE_REQUEST, request.Url.AbsolutePath, out parameters))
            {
                requestDTO.Path = EndPoints.MEDIA_FAVORITE_REQUEST;
                Console.WriteLine($"DELETE: Unmark media ID {parameters["mediaId"]} as favorite");
            }
            else if (RouteMatcher.TryMatch(EndPoints.RATINGS_ID_REQUEST, request.Url.AbsolutePath, out parameters))
            {
                requestDTO.Path = EndPoints.RATINGS_ID_REQUEST;
                Console.WriteLine($"DELETE: Delete rating ID {parameters["ratingId"]}");
            }
            else
            {                
                requestDTO.IsSuccessful = false;
                Console.WriteLine($"=============\nDELETE request to unknown endpoint: {request.Url.AbsolutePath}\n=============");
            }

            requestDTO.Parameters = parameters;
            return requestDTO;
        }
    }
}
