namespace MediaRatingProject.API.Controllers
{
    using MediaRatingProject.API.Requests;
    using MediaRatingProject.Data.Media;
    using MediaRatingProject.Data.StoreInterfaces;
    using MediaRatingProject.Data.Stores;
    using System;
    using System.Text.Json;

    /// <summary>
    /// Class responsible for communicating with the mediaStore
    /// </summary>
    public class MediaController
    {
        private readonly IMediaStore _mediaStore;
        public MediaController(IMediaStore mediaStore)
        {
            _mediaStore = mediaStore;
        }

        /// <summary>
        /// Gets a leaderboard of the highest rated media.
        /// </summary>
        /// <returns>A list of all medias in order.</returns>
        public ResponseHandler GetLeaderboard()
        {
            try
            {
                var mediaList = _mediaStore.GetLeaderboard();
                return ResponseHandler.Ok("Fetched all media successfully.", mediaList);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest($"Error fetching media: {ex.Message}");
            }
        }

        /// <summary>
        /// Searches for media based on search parameters.
        /// </summary>
        /// <param name="request">HTTP request DTO containing the important data.</param>
        /// <returns>A response for the requester with a corresponding message.</returns>
        public ResponseHandler SearchMedia(ParsedRequestDTO request)
        {
            try
            {
                // Query paramemetrs
                request.Parameters.TryGetValue("title", out string? title);
                request.Parameters.TryGetValue("mediaType", out string? mediaType);
                request.Parameters.TryGetValue("sortBy", out string? sortBy);

                // 
                int? releaseYear = null;
                if (request.Parameters.TryGetValue("releaseYear", out string? yearStr) && int.TryParse(yearStr, out int year))
                    releaseYear = year;

                int? ageRestriction = null;
                if (request.Parameters.TryGetValue("ageRestriction", out string? ageStr) && int.TryParse(ageStr, out int age))
                    ageRestriction = age;

                double? minRating = null;
                if (request.Parameters.TryGetValue("rating", out string? ratingStr) && double.TryParse(ratingStr, out double rating))
                    minRating = rating;

                string[]? genres = null;
                if (request.Parameters.TryGetValue("genre", out string? genreStr) && !string.IsNullOrWhiteSpace(genreStr))
                    genres = genreStr.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                // Call the store function
                var mediaList = _mediaStore.SearchMedia(
                    title: title,
                    genres: genres,
                    mediaType: mediaType,
                    releaseYear: releaseYear,
                    ageRestriction: ageRestriction,
                    minRating: minRating,
                    sortBy: sortBy ?? "title",
                    ascending: true
                );

                return ResponseHandler.Ok("Media search results.", mediaList);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest($"Error searching media: {ex.Message}");
            }
        }


        /// <summary>
        /// Gets a media by its id.
        /// </summary>
        /// <param name="request">HTTP request DTO containing the important data.</param>
        /// <returns>A response for the requester with a corresponding message.</returns>
        public ResponseHandler GetMediaById(ParsedRequestDTO request)
        {
            try
            {
                if (!request.Parameters.TryGetValue("mediaId", out var stringId))
                    return ResponseHandler.BadRequest("Missing 'mediaId' parameter.");
                if (!Int32.TryParse(stringId, out int id))
                    return ResponseHandler.BadRequest("Invalid 'mediaId' parameter format.");
                
                var media = _mediaStore.GetMediaById(id);
                if (media == null)
                    return ResponseHandler.NotFound($"Media with '{id}' not found.");

                return ResponseHandler.Ok("Media fetched successfully.", media);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest($"Error fetching media: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new media entry.
        /// </summary>
        /// <param name="request">HTTP request DTO containing the important data.</param>
        /// <returns>A response for the requester with a corresponding message.</returns>
        public ResponseHandler CreateMedia(ParsedRequestDTO request)
        {
            try
            {
                using var doc = JsonDocument.Parse(request.Body);
                var root = doc.RootElement;

                if (!root.TryGetProperty("mediaType", out var mediaTypeProperty))
                    return ResponseHandler.BadRequest("Missing 'mediaType' field.");

                string? mediaType = mediaTypeProperty.GetString()?.ToLower();

                if (string.IsNullOrEmpty(mediaType))
                    return ResponseHandler.BadRequest("Invalid 'mediaType' field.");

                // Instantiate the correct media type.
                BaseMedia? media = mediaType switch
                {
                    "movie" => new MovieMedia(),
                    "series" => new SeriesMedia(),
                    "game" => new GameMedia(),
                    _ => null
                };

                if (media == null)
                    return ResponseHandler.BadRequest($"Unknown mediaType '{mediaType}'.");

                // Populating the data fields.
                media.Title = root.GetProperty("title").GetString() ?? "Untitled";
                media.Description = root.GetProperty("description").GetString() ?? "";
                media.Genres = root.GetProperty("genres").EnumerateArray().Select(g => g.GetString() ?? "").ToList();               
                
                if (!root.TryGetProperty("releaseYear", out var yearPropety) || !yearPropety.TryGetInt32(out int year))
                    return ResponseHandler.BadRequest("Invalid or missing 'releaseYear' field.");
                media.ReleaseYear = year;

                if (!root.TryGetProperty("ageRestriction", out var agePropety) || !agePropety.TryGetInt32(out int age))
                    return ResponseHandler.BadRequest("Invalid or missing 'ageRestriction' field.");
                media.AgeRestriction = age;

                media.UserCreator = request.UserName;
                media.UserId = request.UserID;
                media.MediaType = mediaType;

                // Add to store
                bool success = _mediaStore.CreateMedia(media);
                if (!success)
                    return ResponseHandler.BadRequest("Media Could not be created.");
                
                return ResponseHandler.Ok("Media created successfully.", media);
            }
            catch (JsonException ex)
            {
                return ResponseHandler.BadRequest($"Invalid JSON format: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest($"Error creating media: {ex.Message}");
            }

        }

        /// <summary>
        /// Updates media information.
        /// </summary>
        /// <param name="request">HTTP request DTO containing the important data.</param>
        /// <returns>A response for the requester with a corresponding message.</returns>
        public ResponseHandler UpdateMedia(ParsedRequestDTO request)
        {
            if (!request.Parameters.TryGetValue("mediaId", out var idString))
                return ResponseHandler.BadRequest("Missing mediaId parameter.");

            if (!int.TryParse(idString, out var mediaId))
                return ResponseHandler.BadRequest("Invalid mediaId parameter.");
            
            var existingMedia = _mediaStore.GetMediaById(mediaId);

            if (existingMedia == null)
                return ResponseHandler.NotFound($"Media with ID {mediaId} not found.");

            if (existingMedia.UserId != request.UserID)
                return ResponseHandler.Unauthorized("You are unauthorized to update this media.");

            try
            {
                using var doc = JsonDocument.Parse(request.Body);
                var root = doc.RootElement;
                if (!root.TryGetProperty("mediaType", out var mediaTypePropety))
                    return ResponseHandler.BadRequest("Missing 'mediaType' field.");

                if (mediaTypePropety.GetString() == null)
                    return ResponseHandler.BadRequest("Invalid 'mediaType' field.");

                string? mediaType = mediaTypePropety.GetString()?.ToLower();

                if (string.IsNullOrEmpty(mediaType))
                    return ResponseHandler.BadRequest("Invalid 'mediaType' field.");

                // Serialize to the correct type of media based on mediaType.
                BaseMedia? updatedMedia = mediaType switch
                {
                    "movie" => JsonSerializer.Deserialize<MovieMedia>(request.Body),
                    "series" => JsonSerializer.Deserialize<SeriesMedia>(request.Body),
                    "game" => JsonSerializer.Deserialize<GameMedia>(request.Body),
                    _ => null
                };

                if (updatedMedia == null)
                    return ResponseHandler.BadRequest($"Unknown mediaType '{mediaType}'.");                

                // Copy unchanged values.
                updatedMedia.Id = existingMedia.Id;
                updatedMedia.Ratings = existingMedia.Ratings;
                updatedMedia.FavoriteCount = existingMedia.FavoriteCount;
                updatedMedia.AverageRating = existingMedia.AverageRating;   
                updatedMedia.UserCreator = existingMedia.UserCreator;
                updatedMedia.MediaType = mediaType;
                updatedMedia.UserId = existingMedia.UserId;
                updatedMedia.UserCreator = existingMedia.UserCreator;

                // Populate properties from JSON.
                updatedMedia.Title = root.GetProperty("title").GetString() ?? existingMedia.Title;
                updatedMedia.Description = root.GetProperty("description").GetString() ?? existingMedia.Description;
                updatedMedia.Genres = root.GetProperty("genres").EnumerateArray().Select(g => g.GetString() ?? "").ToList();

                if (!root.TryGetProperty("releaseYear", out var yearProp) || !yearProp.TryGetInt32(out int year))
                    return ResponseHandler.BadRequest("Invalid or missing 'releaseYear' field.");
                updatedMedia.ReleaseYear = year;

                if (!root.TryGetProperty("ageRestriction", out var ageProp) || !ageProp.TryGetInt32(out int age))
                    return ResponseHandler.BadRequest("Invalid or missing 'ageRestriction' field.");
                updatedMedia.AgeRestriction = age;

                if (!_mediaStore.UpdateMedia(updatedMedia, mediaId))
                {
                    return ResponseHandler.BadRequest("Failed to update media.");
                }

                return ResponseHandler.Ok("Media updated successfully.", updatedMedia);
            }
            catch (JsonException ex)
            {
                return ResponseHandler.BadRequest($"Invalid JSON format: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest($"Error updating media: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes media if the requester is the creator.
        /// </summary>
        /// <param name="request">HTTP request DTO containing the important data.</param>
        /// <returns>A response for the requester with a corresponding message.</returns>
        public ResponseHandler DeleteMedia(ParsedRequestDTO request)
        {
            if (!request.Parameters.TryGetValue("mediaId", out var idString))
                return ResponseHandler.BadRequest("Missing mediaId parameter.");

            if (!int.TryParse(idString, out var mediaId))
                return ResponseHandler.BadRequest("Invalid mediaId parameter.");

            var existingMedia = _mediaStore.GetMediaById(mediaId);
            if (existingMedia == null)
                return ResponseHandler.NotFound($"Media with '{mediaId}' not found.");

            if (existingMedia.UserId != request.UserID)
                return ResponseHandler.Unauthorized("You are unauthorized to update this media.");

            if (!_mediaStore.DeleteMedia(mediaId))
            {
                return ResponseHandler.NotFound($"Media with ID {mediaId} not found.");
            }

            return ResponseHandler.Ok("Media {mediaId} deleted successfully.");
        }
    }
}
