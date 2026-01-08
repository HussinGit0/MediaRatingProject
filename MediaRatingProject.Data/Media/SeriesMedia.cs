namespace MediaRatingProject.Data.Media
{
    public class SeriesMedia : BaseMedia
    {
        public SeriesMedia() { }

        public SeriesMedia(int id, string title, string description, List<string> genres, int year, int ageRestriction) : base(id, title, description, genres, year, ageRestriction)
        {
        }
    }
}
