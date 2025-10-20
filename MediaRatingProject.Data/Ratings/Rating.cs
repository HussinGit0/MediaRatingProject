namespace MediaRatingProject.Data.Ratings
{
    using MediaRatingProject.Data.Media;
    using MediaRatingProject.Data.Users;
    public class Rating
    {
        public int Id { get; set; }
        public BaseUser User { get; set; }
        public BaseMedia Media { get; set; }
        public bool Approved { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }

        public Dictionary<BaseUser, bool> Likes { get; set; } // True = Like, False = Dislike
    }
}
