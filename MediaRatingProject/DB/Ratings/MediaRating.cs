using MediaRatingProject.DB.MediaTypes;
using MediaRatingProject.DB.Users;

namespace MediaRatingProject.DB.Ratings
{
    public class MediaRating
    {
        public int Id { get; set; }
        public BaseUser User { get; set; }
        public MediaEntry Media { get; set; }
        public bool Approved { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }

        public Dictionary<BaseUser, bool> Likes { get; set; } // True = Like, False = Dislike
    }
}
