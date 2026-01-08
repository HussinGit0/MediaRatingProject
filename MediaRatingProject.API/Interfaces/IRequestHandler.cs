namespace MediaRatingProject.API.Interfaces
{
    using MediaRatingProject.API.Requests;

    public interface IRequestHandler
    {
        public ResponseHandler HandleRequest(ParsedRequestDTO request);
    }
}
