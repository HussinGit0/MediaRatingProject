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

        public int LikeCount { get; set; }
        public List<BaseUser> Likes { get; set; } // Users who liked/disliked the review
    }
}