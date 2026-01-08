namespace MediaRatingProject.API.Controllers
{
    using MediaRatingProject.API.Interfaces;
    using MediaRatingProject.API.Requests;
    using MediaRatingProject.Data.StoreInterfaces;
    using MediaRatingProject.Data.Stores;

    /// <summary>
    /// Class responsible for communicating with the favorite store.
    /// </summary>
    public class FavoriteController: IFavoriteController
    {
        private readonly IFavoriteStore _favoriteStore;

        public FavoriteController(IFavoriteStore store)
        {
            _favoriteStore = store;
        }

        /// <summary>
        /// Gets a user by its id.
        /// </summary>
        /// <param name="request">HTTP request DTO containing the important data.</param>
        /// <returns>A response for the requester with a corresponding message.</returns>
        public ResponseHandler GetFavoritesByUserID(ParsedRequestDTO request)
        {
            try
            {
                if (request.UserID == null)
                    return ResponseHandler.BadRequest("Invalid or missing user ID.");

                var favorites = _favoriteStore.GetFavoritesByUserID(request.UserID);
                return ResponseHandler.Ok("Fetched favorites successfully.", favorites);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest($"Error fetching favorites: {ex.Message}");
            }
        }

        /// <summary>
        /// Marks a media as favorite for the user.
        /// </summary>
        /// <param name="request">HTTP request DTO containing the important data.</param>
        /// <returns>A response for the requester with a corresponding message.</returns>
        public ResponseHandler MarkFavorite(ParsedRequestDTO request)
        {
            try
            {
                if (request.UserID == null)
                    return ResponseHandler.BadRequest("Invalid or missing user ID.");

                if (!request.Parameters.TryGetValue("mediaId", out var mediaIdStr) || !int.TryParse(mediaIdStr, out int mediaId))
                    return ResponseHandler.BadRequest("Invalid or missing media ID.");

                bool success = _favoriteStore.MarkFavorite(request.UserID, mediaId);
                if (success)
                    return ResponseHandler.Ok("Media marked as favorite.");

                return ResponseHandler.BadRequest("Media already marked as favorite or could not be added.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest($"Error marking favorite: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles unmarking a media as favorite.
        /// </summary>
        /// <param name="request">HTTP request DTO containing the important data.</param>
        /// <returns>A response for the requester with a corresponding message.</returns>
        public ResponseHandler UnmarkFavorite(ParsedRequestDTO request)
        {
            try
            {
                if (request.UserID == null)
                    return ResponseHandler.BadRequest("Invalid or missing user ID.");

                if (!request.Parameters.TryGetValue("mediaId", out var mediaIdStr) || !int.TryParse(mediaIdStr, out int mediaId))
                    return ResponseHandler.BadRequest("Invalid or missing media ID.");

                bool success = _favoriteStore.UnmarkFavorite(request.UserID, mediaId);
                if (success)
                    ResponseHandler.Ok("Media unmarked as favorite.");
                
                return ResponseHandler.NotFound("Favorite not found or could not be removed.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest($"Error unmarking favorite: {ex.Message}");
            }
        }
    }
}
