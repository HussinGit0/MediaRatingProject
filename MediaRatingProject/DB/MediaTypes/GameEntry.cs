namespace MediaRatingProject.DB.MediaTypes
{
    using MediaRatingProject.Enums;

    internal class GameEntry: MediaEntry
    {
        public GameEntry(int id,
                  string title,
                  string genre,
                  DateTime year,
                  AgeRestriction ageRestriction) : base(id, title, genre, year, ageRestriction) { }
    }
}
