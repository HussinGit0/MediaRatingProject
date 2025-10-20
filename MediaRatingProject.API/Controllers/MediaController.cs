namespace MediaRatingProject.API.Controllers
{
    using MediaRatingProject.API.Requests;
    using MediaRatingProject.Data.Media;
    using MediaRatingProject.Data.Stores;
    using System;
    using System.Text.Json;

    public class MediaController
    {
        private readonly MediaStore _mediaStore;

        public MediaController(MediaStore mediaStore)
        {
            _mediaStore = mediaStore;
        }

        public ResponseHandler GetAllMedia()
        {
            try
            {
                var mediaList = _mediaStore.GetAllMedia();
                var json = JsonSerializer.Serialize(mediaList);
                return ResponseHandler.Ok("Fetched all media successfully.", json);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest($"Error fetching media: {ex.Message}");
            }
        }

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

                var json = JsonSerializer.Serialize(media);
                return ResponseHandler.Ok("Media fetched successfully.", json);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest($"Error fetching media: {ex.Message}");
            }
        }

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

                // This is done manually, and will be automated in the future.
                media.Title = root.GetProperty("title").GetString() ?? "Untitled";
                media.Description = root.GetProperty("description").GetString() ?? "";
                media.Genres = root.GetProperty("genres").EnumerateArray().Select(g => g.GetString() ?? "").ToArray();

                if (!root.TryGetProperty("releaseYear", out var yearPropety) || !yearPropety.TryGetInt32(out int year))
                    return ResponseHandler.BadRequest("Invalid or missing 'releaseYear' field.");
                media.ReleaseYear = year;

                if (!root.TryGetProperty("ageRestriction", out var agePropety) || !agePropety.TryGetInt32(out int age))
                    return ResponseHandler.BadRequest("Invalid or missing 'ageRestriction' field.");
                media.AgeRestriction = age;

                // Add to store
                bool success = _mediaStore.AddMedia(media);
                if (!success)
                    return ResponseHandler.BadRequest("A media with the same title already exists.");

                return ResponseHandler.Ok("Media created successfully.", JsonSerializer.Serialize(media));
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

        public ResponseHandler UpdateMedia(ParsedRequestDTO request)
        {
            if (!request.Parameters.TryGetValue("mediaId", out var idString))
                return ResponseHandler.BadRequest("Missing mediaId parameter.");

            if (!int.TryParse(idString, out var mediaId))
                return ResponseHandler.BadRequest("Invalid mediaId parameter.");

            var existingMedia = _mediaStore.GetMediaById(mediaId);

            if (existingMedia == null)
                return ResponseHandler.NotFound($"Media with ID {mediaId} not found.");

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
                updatedMedia.FavoritedBy = existingMedia.FavoritedBy;
                updatedMedia.AverageRating = existingMedia.AverageRating;   

                // Populate properties from JSON.
                updatedMedia.Title = root.GetProperty("title").GetString() ?? existingMedia.Title;
                updatedMedia.Description = root.GetProperty("description").GetString() ?? existingMedia.Description;
                updatedMedia.Genres = root.GetProperty("genres").EnumerateArray()
                    .Select(g => g.GetString() ?? "").ToArray();

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

                return ResponseHandler.Ok("Media updated successfully.", JsonSerializer.Serialize(updatedMedia));
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

        public ResponseHandler DeleteMedia(ParsedRequestDTO request)
        {
            if (!request.Parameters.TryGetValue("mediaId", out var idString))
                return ResponseHandler.BadRequest("Missing mediaId parameter.");

            if (!int.TryParse(idString, out var mediaId))
                return ResponseHandler.BadRequest("Invalid mediaId parameter.");

            if (!_mediaStore.RemoveMedia(mediaId))
            {
                return ResponseHandler.NotFound($"Media with ID {mediaId} not found.");
            }

            return ResponseHandler.Ok("Media {mediaId} deleted successfully.");
        }
    }
}
