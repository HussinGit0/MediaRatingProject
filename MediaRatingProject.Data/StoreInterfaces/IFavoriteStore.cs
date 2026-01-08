namespace MediaRatingProject.Data.StoreInterfaces
{
    using MediaRatingProject.Data.Media;

    public interface IFavoriteStore
    {
        public List<BaseMedia> GetFavoritesByUserID(int? userId);
        public bool MarkFavorite(int? userId, int mediaId);
        public bool UnmarkFavorite(int? userId, int mediaId);

    }
}
