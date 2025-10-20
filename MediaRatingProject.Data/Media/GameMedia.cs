namespace MediaRatingProject.Data.Media
{
    public class GameMedia : BaseMedia
    {
        public GameMedia() { }

        public GameMedia(int id, string title, string description, string[] genres, int year, int ageRestriction) : base(id, title, description, genres, year, ageRestriction)
        {
        }
    }
}
