namespace MediaRatingProject.API.Interfaces
{
    using MediaRatingProject.API.Requests;

    public interface IFavoriteController
    {
        public ResponseHandler GetFavoritesByUserID(ParsedRequestDTO request);
        public ResponseHandler MarkFavorite(ParsedRequestDTO request);
        public ResponseHandler UnmarkFavorite(ParsedRequestDTO request);
    }
}
