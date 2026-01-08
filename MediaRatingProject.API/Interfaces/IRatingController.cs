namespace MediaRatingProject.API.Interfaces
{
    using MediaRatingProject.API.Requests;

    public interface IRatingController
    {
        public ResponseHandler GetRatingsByUser(ParsedRequestDTO request);
        public ResponseHandler CreateRating(ParsedRequestDTO request);
        public ResponseHandler UpdateRating(ParsedRequestDTO request);
        public ResponseHandler LikeRating(ParsedRequestDTO request);
        public ResponseHandler ApproveRating(ParsedRequestDTO request);
    }
}
