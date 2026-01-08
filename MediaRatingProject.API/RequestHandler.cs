using MediaRatingProject.API.Controllers;
using MediaRatingProject.API.Interfaces;
using MediaRatingProject.API.Requests;
using MediaRatingProject.API.Services;

namespace MediaRatingProject.API
{
    public class RequestHandler
    {
        private readonly UsersController _usersController;
        private readonly MediaController _mediaController;
        private readonly FavoriteController _favoriteController;
        private readonly RatingController _ratingController;
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestHandler"/> class.
        /// </summary>
        /// <param name="usersController">A class responsible for handling users.</param>
        /// <param name="mediaController">A class responsible for handling media entries.</param>
        public RequestHandler(
            UsersController usersController,
            MediaController mediaController,
            FavoriteController favoriteController,
            RatingController ratingController,
            ITokenService tokenService)
        {
            _usersController = usersController;
            _mediaController = mediaController;
            _favoriteController = favoriteController;
            _ratingController = ratingController;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Processes the incoming parsed request and routes it to the appropriate controller method.
        /// </summary>
        /// <param name="request">A parsed HTTP request.</param>
        /// <returns>A response handler.</returns>
        public ResponseHandler HandleRequest(ParsedRequestDTO request)
        {
            if (!request.IsSuccessful)
            {
                Console.WriteLine("Invalid request, cannot handle.");
                return ResponseHandler.BadRequest("Bad request! Could not parse.");
            }

            if (!Authenticate(request, out string username))
                return ResponseHandler.Unauthorized("Unauthorized or missing token.");

            request.UserName = username; // This saves the trouble of parsing the token again in each controller.

            request.UserID = _usersController.GetUserIdByUsername(request); // Preload user data for controllers that might need it.

            switch (request.HttpMethod)
            {
                case "POST":
                    return HandlePOSTRequest(request);
                    
                case "GET":
                    return HandleGETRequest(request);
                    
                case "PUT":
                    return HandlePUTRequest(request);
                    
                case "DELETE":
                    return HandleDELETERequest(request);
                    
                default:
                    Console.WriteLine("Unsupported HTTP method.");
                    return ResponseHandler.BadRequest("Unsupported HTTP method.");
                    
            }
        }

        /// <summary>
        /// Handles POST requests.
        /// </summary>
        /// <param name="request">A parsed HTTP request.</param>
        /// <returns>A response handler.</returns>
        private ResponseHandler HandlePOSTRequest(ParsedRequestDTO request)
        {
            string username; 
            switch (request.Path)
            {
                case EndPoints.USERS_LOGIN_REQUEST:                                        
                    return _usersController.Login(request);
                    

                case EndPoints.USERS_REGISTER_REQUEST:                    
                    return _usersController.Register(request);
                    

                case EndPoints.MEDIA_REQUEST:
                    return _mediaController.CreateMedia(request);
                    

                case EndPoints.MEDIA_RATE_REQUEST:
                    return _ratingController.CreateRating(request);


                case EndPoints.MEDIA_FAVORITE_REQUEST:
                    return _favoriteController.MarkFavorite(request);


                case EndPoints.MEDIA_LIKE_REQUEST:
                    return _ratingController.LikeRating(request);


                case EndPoints.RATINGS_ID_CONFIRM_REQUEST:
                    return _ratingController.ApproveRating(request);


                default:
                    Console.WriteLine($"POST request to unknown path: {request.Path}");
                    return ResponseHandler.BadRequest("Unknown POST request path.");
                    
            }
        }

        /// <summary>
        /// Handles GET requests.
        /// </summary>
        /// <param name="request">A parsed HTTP request.</param>
        /// <returns>A response handler.</returns>
        private ResponseHandler HandleGETRequest(ParsedRequestDTO request)
        {
            switch (request.Path)
            {
                case EndPoints.USERS_PROFILE_REQUEST:
                    return _usersController.GetUserByID(request);
                //ResponseHandler.Ok("Get user profile endpoint hit.");


                case EndPoints.USERS_RATINGS_REQUEST:
                    return _ratingController.GetRatingsByUser(request);


                case EndPoints.USERS_FAVORITES_REQUEST:
                    return _favoriteController.GetFavoritesByUserID(request);


                case EndPoints.USERS_RECOMMENDATION_REQUEST:
                    return ResponseHandler.Ok("Get user recommendations endpoint hit (NOT IMPLEMENTED).");
                    

                case EndPoints.MEDIA_ID_REQUEST:                    
                    return _mediaController.GetMediaById(request);
                    

                case EndPoints.MEDIA_REQUEST:
                    return _mediaController.SearchMedia(request);


                case EndPoints.LEADERBOARD_REQUEST:
                    return _mediaController.GetLeaderboard();


                default:
                    Console.WriteLine($"GET request to unknown path: {request.Path}");
                    return ResponseHandler.BadRequest("Unknown GET request path.");
                    
            }
        }

        /// <summary>
        /// Handles PUT requests.
        /// </summary>
        /// <param name="request">A parsed HTTP request.</param>
        /// <returns>A response handler.</returns>
        private ResponseHandler HandlePUTRequest(ParsedRequestDTO request)
        {
            switch (request.Path)
            {
                case EndPoints.USERS_PROFILE_REQUEST:
                    return ResponseHandler.Ok("Update profile endpoint hit.");
                    

                case EndPoints.MEDIA_ID_REQUEST:
                    return _mediaController.UpdateMedia(request);
                    

                case EndPoints.RATINGS_ID_REQUEST:
                    return _ratingController.UpdateRating(request); 


                default:
                    Console.WriteLine($"PUT request to unknown path: {request.Path}");
                    return ResponseHandler.BadRequest("Unknown PUT request path.");
                    
            }
        }

        /// <summary>
        /// Handles DELETE requests.
        /// </summary>
        /// <param name="request">A parsed HTTP request.</param>
        /// <returns>A response handler.</returns>
        private ResponseHandler HandleDELETERequest(ParsedRequestDTO request)
        {           
            switch (request.Path)
            {
                case EndPoints.MEDIA_ID_REQUEST:
                    return _mediaController.DeleteMedia(request);
                    

                case EndPoints.MEDIA_FAVORITE_REQUEST: 
                    return _favoriteController.UnmarkFavorite(request);


                case EndPoints.RATINGS_ID_REQUEST:
                    return ResponseHandler.Ok("Delete rating endpoint hit.");   
                    

                default:
                    Console.WriteLine($"DELETE request to unknown path: {request.Path}");
                    return ResponseHandler.BadRequest("Unknown DELETE request path.");
                    
            }
        }

        private bool Authenticate(ParsedRequestDTO request, out string username)
        {
            username = null;

            // Allow public endpoints
            if (request.Path == EndPoints.USERS_LOGIN_REQUEST ||
                request.Path == EndPoints.USERS_REGISTER_REQUEST)
                return true;

            // Public media GETs
            if (request.HttpMethod == "GET" &&
                    (request.Path == EndPoints.MEDIA_REQUEST ||
                     request.Path == EndPoints.MEDIA_ID_REQUEST ||
                     request.Path == EndPoints.LEADERBOARD_REQUEST))
                return true;


            return _tokenService.ValidateToken(request.Token, out username);
        }
    }
}
