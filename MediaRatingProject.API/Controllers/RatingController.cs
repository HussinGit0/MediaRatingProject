namespace MediaRatingProject.API.Controllers
{
    using MediaRatingProject.API.Requests;
    using MediaRatingProject.Data.Media;
    using MediaRatingProject.Data.Ratings;
    using MediaRatingProject.Data.Stores;
    using MediaRatingProject.Data.Users;
    using System;
    using System.Text.Json;

    public class RatingController
    {
        private readonly RatingStore _ratingStore;

        public RatingController(RatingStore ratingStore)
        {
            _ratingStore = ratingStore;
        }

        /// <summary>
        /// Gets all ratings made by a user.
        /// </summary>
        /// <param name="request">HTTP request DTO containing the important data.</param>
        /// <returns>A response for the requester with a corresponding message.</returns>
        public ResponseHandler GetRatingsByUser(ParsedRequestDTO request)
        {
            try
            {
                if (!request.Parameters.TryGetValue("userId", out var stringId))
                    return ResponseHandler.BadRequest("Missing 'userId' parameter.");
                if (!Int32.TryParse(stringId, out int userId))
                    return ResponseHandler.BadRequest("Invalid 'userId' parameter format.");
                var ratings = _ratingStore.GetRatingsByUser(userId);
                return ResponseHandler.Ok("Fetched ratings successfully.", ratings);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest($"Error fetching ratings: {ex.Message}");
            }
        }


        /// <summary>
        /// Creates a rating for a media by a user.
        /// </summary>
        /// <param name="request">HTTP request DTO containing the important data.</param>
        /// <returns>A response for the requester with a corresponding message.</returns>
        public ResponseHandler CreateRating(ParsedRequestDTO request)
        {
            if (request.UserID == null)
                return ResponseHandler.BadRequest("Invalid or missing user ID.");

            try
            {
                using var doc = JsonDocument.Parse(request.Body);
                var root = doc.RootElement;

                if (!request.Parameters.TryGetValue("mediaId", out var stringId))
                    return ResponseHandler.BadRequest("Missing 'mediaId' parameter.");

                if (!Int32.TryParse(stringId, out int mediaId))
                    return ResponseHandler.BadRequest("Invalid 'mediaId' parameter format.");


                if (!root.TryGetProperty("stars", out var ratingValueElement) || !ratingValueElement.TryGetInt32(out int ratingValue))
                    return ResponseHandler.BadRequest("Invalid or missing 'stars'.");

                if (ratingValue < 1 || ratingValue > 5)
                    return ResponseHandler.BadRequest("Rating value must be between 1 and 5.");

                string? comment = null;
                if (root.TryGetProperty("comment", out var commentElement) && commentElement.ValueKind == JsonValueKind.String)
                {
                    comment = commentElement.GetString();
                }

                var rating = new Rating
                {
                    User = new User { Id = request.UserID.Value, Username = request.UserName },
                    Media = new MediaSummaryDTO { Id = mediaId },
                    Score = ratingValue,
                    Comment = comment,
                    Approved = false, // New ratings are not approved by default
                    LikeCount = 0
                };

                bool success = _ratingStore.CreateRating(rating);
                if (!success)
                    return ResponseHandler.BadRequest("Failed to create rating.");
                return ResponseHandler.Ok("Rating created successfully.");
            }
            catch (JsonException ex)
            {
                return ResponseHandler.BadRequest($"Invalid JSON format: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest($"Error creating rating: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing rating made by a user, provided the IDs match.
        /// </summary>
        /// <param name="request">HTTP request DTO containing the important data.</param>
        /// <returns>A response for the requester with a corresponding message.</returns>
        public ResponseHandler UpdateRating(ParsedRequestDTO request)
        {
            try
            {
                // Parse JSON body
                using var doc = JsonDocument.Parse(request.Body);
                var root = doc.RootElement;

                // Extract required fields             
                if (!request.Parameters.TryGetValue("ratingId", out var stringId))
                    return ResponseHandler.BadRequest("Missing 'mediaId' parameter.");

                if (!Int32.TryParse(stringId, out int ratingId))
                    return ResponseHandler.BadRequest("Invalid 'ratingId' parameter format.");

                if (!root.TryGetProperty("stars", out var ratingValueElement) || !ratingValueElement.TryGetInt32(out int ratingValue))
                    return ResponseHandler.BadRequest("Invalid or missing 'stars'.");

                string? comment = null;
                if (root.TryGetProperty("comment", out var commentElement) && commentElement.ValueKind == JsonValueKind.String)
                {
                    comment = commentElement.GetString();
                }

                var userId = request.UserID;
                if (userId == null)
                    return ResponseHandler.BadRequest("Invalid or missing user ID.");

                // Construct the updated rating object
                var rating = new Rating
                {
                    Id = ratingId,
                    User = new User { Id = userId.Value, Username = request.UserName },
                    Score = ratingValue,
                    Comment = comment,
                    Approved = false // By default, updated ratings might require re-approval if comment changed
                };

                // Call the store function
                bool success = _ratingStore.UpdateRating(rating);
                if (!success)
                    return ResponseHandler.BadRequest("Failed to update rating. Make sure the rating exists and belongs to the user.");

                return ResponseHandler.Ok("Rating updated successfully.");
            }
            catch (JsonException ex)
            {
                return ResponseHandler.BadRequest($"Invalid JSON format: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest($"Error updating rating: {ex.Message}");
            }
        }

        /// <summary>
        /// Addds a like to a rating.
        /// </summary>
        /// <param name="request">HTTP request DTO containing the important data.</param>
        /// <returns>A response for the requester with a corresponding message.</returns>
        public ResponseHandler LikeRating(ParsedRequestDTO request)
        {
            // Extract required fields             
            if (!request.Parameters.TryGetValue("ratingId", out var stringId))
                return ResponseHandler.BadRequest("Missing 'mediaId' parameter.");

            if (!Int32.TryParse(stringId, out int ratingId))
                return ResponseHandler.BadRequest("Invalid 'ratingId' parameter format.");

            // Call the store function
            bool success = _ratingStore.LikeRating(ratingId, request.UserID.Value);
            if (!success)
                return ResponseHandler.BadRequest("Failed to update rating. Make sure the rating exists and you haven't liked it already.");

            return ResponseHandler.Ok("Rating liked successfully.");
        }

        /// <summary>
        /// Approves a rating if the requester has the same ID as the media's creator.
        /// </summary>
        /// <param name="request">HTTP request DTO containing the important data.</param>
        /// <returns>A response for the requester with a corresponding message.</returns>
        public ResponseHandler ApproveRating(ParsedRequestDTO request)
        {
            // Extract required fields             
            if (!request.Parameters.TryGetValue("ratingId", out var stringId))
                return ResponseHandler.BadRequest("Missing 'mediaId' parameter.");

            if (!Int32.TryParse(stringId, out int ratingId))
                return ResponseHandler.BadRequest("Invalid 'ratingId' parameter format.");

            // Call the store function
            bool success = _ratingStore.ApproveRating(ratingId, request.UserID.Value);
            if (!success)
                return ResponseHandler.BadRequest("Failed to update rating. Make sure the rating exists and you haven't liked it already.");

            return ResponseHandler.Ok("Comment approved successfully.");
        }
    }
}
