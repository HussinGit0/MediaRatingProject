using MediaRatingProject.DB.MediaTypes;
using MediaRatingProject.DB.Users;

namespace MediaRatingProject.DB.Ratings
{
    public class Favorite
    {
        public BaseUser User { get; set; }
        public MediaEntry Media { get; set; }
    }
}
