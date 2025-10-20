namespace MediaRatingProject.Data.Ratings
{
    using MediaRatingProject.Data.Media;
    using MediaRatingProject.Data.Users;
    public class Favorite
    {
        public BaseUser User { get; set; }
        public BaseMedia Media { get; set; }
    }
}
