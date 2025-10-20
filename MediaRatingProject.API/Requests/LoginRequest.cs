using MediaRatingProject.API.Interfaces;

namespace MediaRatingProject.API.Requests
{
    public class LoginPOSTRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public void Accept(IRequestVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
