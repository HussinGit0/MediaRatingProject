using MediaRatingProject.API.Controllers;
using MediaRatingProject.API.Requests;
using System;

namespace MediaRatingProject.API
{
    public class RequestHandler
    {
        private readonly UsersController _usersController;
        private readonly MediaController _mediaController;

        public RequestHandler(
            UsersController usersController,
            MediaController mediaController)
        {
            _usersController = usersController;
            _mediaController = mediaController;
        }

        public ResponseHandler HandleRequest(ParsedRequestDTO request)
        {
            if (!request.IsSuccessful)
            {
                Console.WriteLine("Invalid request, cannot handle.");
                return ResponseHandler.BadRequest("Bad request! Could not parse.");
            }

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

        private ResponseHandler HandlePOSTRequest(ParsedRequestDTO request)
        {            
            switch (request.Path)
            {
                case EndPoints.USERS_LOGIN_REQUEST:                    
                    return _usersController.Login(request);
                    

                case EndPoints.USERS_REGISTER_REQUEST:                    
                    return _usersController.Register(request);
                    

                // ---- MEDIA ACTIONS ----
                case EndPoints.MEDIA_REQUEST:
                    return _mediaController.CreateMedia(request);
                    

                case EndPoints.MEDIA_RATE_REQUEST:
                    return ResponseHandler.Ok("Rate media endpoint hit.");
                    

                case EndPoints.MEDIA_FAVORITE_REQUEST:
                    return ResponseHandler.Ok("Favorite media endpoint hit.");
                    

                case EndPoints.MEDIA_LIKE_REQUEST:
                    return ResponseHandler.Ok("Like rating endpoint hit.");
                    

                case EndPoints.RATINGS_ID_CONFIRM_REQUEST:
                    return ResponseHandler.Ok("Confirm rating endpoint hit.");
                    

                default:
                    Console.WriteLine($"POST request to unknown path: {request.Path}");
                    return ResponseHandler.BadRequest("Unknown POST request path.");
                    
            }
        }

        private ResponseHandler HandleGETRequest(ParsedRequestDTO request)
        {
            switch (request.Path)
            {
                case EndPoints.USERS_PROFILE_REQUEST:
                    return ResponseHandler.Ok("Get user profile endpoint hit.");
                    

                case EndPoints.USERS_RATINGS_REQUEST:
                    return ResponseHandler.Ok("Get user ratings endpoint hit.");
                    

                case EndPoints.USERS_FAVORITES_REQUEST:
                    return ResponseHandler.Ok("Get user favorites endpoint hit.");
                    

                case EndPoints.USERS_RECOMMENDATION_REQUEST:
                    return ResponseHandler.Ok("Get user recommendations endpoint hit.");
                    

                case EndPoints.MEDIA_ID_REQUEST:                    
                    return _mediaController.GetMediaById(request);
                    

                case EndPoints.MEDIA_REQUEST:
                    return _mediaController.GetAllMedia();
                    

                case EndPoints.LEADERBOARD_REQUEST:
                    return ResponseHandler.Ok("Get leaderboard endpoint hit.");
                    

                default:
                    Console.WriteLine($"GET request to unknown path: {request.Path}");
                    return ResponseHandler.BadRequest("Unknown GET request path.");
                    
            }
        }

        private ResponseHandler HandlePUTRequest(ParsedRequestDTO request)
        {
            switch (request.Path)
            {
                case EndPoints.USERS_PROFILE_REQUEST:
                    return ResponseHandler.Ok("Update profile endpoint hit.");
                    

                case EndPoints.MEDIA_ID_REQUEST:
                    return _mediaController.UpdateMedia(request);
                    

                case EndPoints.RATINGS_ID_REQUEST:
                    return ResponseHandler.Ok("Update rating endpoint hit.");
                    

                default:
                    Console.WriteLine($"PUT request to unknown path: {request.Path}");
                    return ResponseHandler.BadRequest("Unknown PUT request path.");
                    
            }
        }

        private ResponseHandler HandleDELETERequest(ParsedRequestDTO request)
        {           
            switch (request.Path)
            {
                case EndPoints.MEDIA_ID_REQUEST:
                    return _mediaController.DeleteMedia(request);
                    

                case EndPoints.MEDIA_FAVORITE_REQUEST: 
                    return ResponseHandler.Ok("Unfavorite media endpoint hit.");
                    

                case EndPoints.RATINGS_ID_REQUEST:
                    return ResponseHandler.Ok("Delete rating endpoint hit.");   
                    

                default:
                    Console.WriteLine($"DELETE request to unknown path: {request.Path}");
                    return ResponseHandler.BadRequest("Unknown DELETE request path.");
                    
            }
        }
    }
}
