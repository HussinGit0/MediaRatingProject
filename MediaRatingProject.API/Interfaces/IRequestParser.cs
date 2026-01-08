namespace MediaRatingProject.API.Interfaces
{
    using MediaRatingProject.API.Requests;
    using System.Net;

    public interface IRequestParser
    {
        public ParsedRequestDTO ParseRequest(HttpListenerRequest request, string body);
    }
}
