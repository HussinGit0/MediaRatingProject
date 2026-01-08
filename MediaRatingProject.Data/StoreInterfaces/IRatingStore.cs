namespace MediaRatingProject.Data.StoreInterfaces
{

    using MediaRatingProject.Data.Ratings;

    public interface IRatingStore
    {
        public List<Rating> GetRatingsByUser(int userId);

        public bool CreateRating(Rating rating);

        public bool UpdateRating(Rating updatedRating);
        public bool LikeRating(int ratingId, int userId);
        public bool ApproveRating(int ratingId, int approverId);
    }
}
