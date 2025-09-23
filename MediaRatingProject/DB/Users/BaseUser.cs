
namespace MediaRatingProject.DB.Users
{
    using MediaRatingProject.DB.Ratings;
    using System.Security;

    public abstract class BaseUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<MediaRating> RatedMedia { get; set; }
        public List<Favorite> FavoriteMedia { get; set; }

        public BaseUser(string name, string password)
        {
            this.Username = name;
            this.Password = password;
        }
    }
}
