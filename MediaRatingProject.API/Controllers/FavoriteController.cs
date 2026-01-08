using MediaRatingProject.API.Requests;
using MediaRatingProject.Data.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingProject.API.Controllers
{
    public class FavoriteController
    {
        private readonly FavoriteStore _favoriteStore;

        public FavoriteController(FavoriteStore store)
        {
            _favoriteStore = store;
        }

        /// <summary>
        /// Handles marking a media as favorite.
        /// </summary>
        public ResponseHandler MarkFavorite(ParsedRequestDTO request)
        {
            try
            {
                if (request.UserID == null)
                    return ResponseHandler.BadRequest("Invalid or missing user ID.");

                if (!request.Parameters.TryGetValue("mediaId", out var mediaIdStr) || !int.TryParse(mediaIdStr, out int mediaId))
                    return ResponseHandler.BadRequest("Invalid or missing media ID.");

                bool success = _favoriteStore.MarkFavorite(request.UserID, mediaId);
                return success ? ResponseHandler.Ok("Media marked as favorite.")
                               : ResponseHandler.BadRequest("Media already marked as favorite or could not be added.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest($"Error marking favorite: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles unmarking a media as favorite.
        /// </summary>
        public ResponseHandler UnmarkFavorite(ParsedRequestDTO request)
        {
            try
            {
                if (request.UserID == null)
                    return ResponseHandler.BadRequest("Invalid or missing user ID.");

                if (!request.Parameters.TryGetValue("mediaId", out var mediaIdStr) || !int.TryParse(mediaIdStr, out int mediaId))
                    return ResponseHandler.BadRequest("Invalid or missing media ID.");

                bool success = _favoriteStore.UnmarkFavorite(request.UserID, mediaId);
                return success ? ResponseHandler.Ok("Media unmarked as favorite.")
                               : ResponseHandler.NotFound("Favorite not found or could not be removed.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest($"Error unmarking favorite: {ex.Message}");
            }
        }
    }
}
