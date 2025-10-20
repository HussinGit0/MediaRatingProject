namespace MediaRatingProject.Data.Media
{
    public class MovieMedia : BaseMedia
    {
        public MovieMedia() { }

        public MovieMedia(int id, string title, string description, string[] genres, int year, int ageRestriction) : base(id, title, description, genres, year, ageRestriction)
        {
        }
    }
}
