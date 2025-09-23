namespace MediaRatingProject.DB.MediaTypes
{
    using MediaRatingProject.Enums;

    public class MovieEntry : MediaEntry
    {
        public MovieEntry(int id,
                          string title,
                          string genre,
                          DateTime year,
                          AgeRestriction ageRestriction) : base(id, title, genre, year, ageRestriction) { }
    }
}

