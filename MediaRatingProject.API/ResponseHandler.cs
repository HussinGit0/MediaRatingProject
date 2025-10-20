
namespace MediaRatingProject.API
{
    /// <summary>
    /// A class representing a response for HTTP requests.
    /// </summary>
    public class ResponseHandler
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string? Body { get; set; }

        public static ResponseHandler Ok(string message, string? body = null)
            => new ResponseHandler { StatusCode = 200, Message = message, Body = body };

        public static ResponseHandler Unauthorized(string message)
            => new ResponseHandler { StatusCode = 401, Message = message };

        public static ResponseHandler NotFound(string message)
            => new ResponseHandler { StatusCode = 404, Message = message };

        public static ResponseHandler BadRequest(string message)
            => new ResponseHandler { StatusCode = 400, Message = message };
    }
}
