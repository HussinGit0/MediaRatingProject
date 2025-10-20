namespace MediaRatingProject.API.Interfaces
{
    public interface IRequest
    {
        void Accept(IRequestVisitor visitor);
    }
}
