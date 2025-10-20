using MediaRatingProject.API.Requests;

namespace MediaRatingProject.API.Interfaces
{
    public interface IRequestVisitor
    {
        void Visit(LoginPOSTRequest request);
        void Visit(RegisterPOSTRequest request);
        void Visit(FailedRequest request);

    }
}
