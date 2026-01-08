namespace MediaRatingProject.API.Interfaces
{
    using MediaRatingProject.API.Requests;

    public interface IUsersController
    {
        public ResponseHandler Register(ParsedRequestDTO request);
        public ResponseHandler Login(ParsedRequestDTO request);
        public ResponseHandler GetUserByID(ParsedRequestDTO request);
        public int? GetUserIdByUsername(ParsedRequestDTO request);
    }
}
