namespace MediaRatingProject.Data.StoreInterfaces
{
    using MediaRatingProject.Data.Media;

    public interface IMediaStore
    {
        public bool CreateMedia(BaseMedia media);
        public BaseMedia GetMediaById(int id);
        public bool DeleteMedia(int id);
        public List<MediaSummaryDTO> GetLeaderboard();
        public bool UpdateMedia(BaseMedia updatedMedia, int id);
        public List<MediaSummaryDTO> SearchMedia(string? title = null,
                                         string[]? genres = null,
                                         string? mediaType = null,
                                         int? releaseYear = null,
                                         int? ageRestriction = null,
                                         double? minRating = null,
                                         string sortBy = "title",
                                         bool ascending = true);                
    }
}
