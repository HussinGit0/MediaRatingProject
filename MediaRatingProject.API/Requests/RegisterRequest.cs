using MediaRatingProject.API.Interfaces;

namespace MediaRatingProject.API.Requests
{
    public class RegisterPOSTRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public void Accept(IRequestVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
