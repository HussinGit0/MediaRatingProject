namespace MediaRatingProject.API.Interfaces
{
    using MediaRatingProject.API.Requests;

    public interface IMediaController
    {
        public ResponseHandler GetLeaderboard();

        public ResponseHandler SearchMedia(ParsedRequestDTO request);

        public ResponseHandler GetMediaById(ParsedRequestDTO request);
        public ResponseHandler CreateMedia(ParsedRequestDTO request);
        public ResponseHandler UpdateMedia(ParsedRequestDTO request);
        public ResponseHandler DeleteMedia(ParsedRequestDTO request);
    }
}
