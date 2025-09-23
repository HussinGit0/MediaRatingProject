namespace MediaRatingProject.DB.MediaTypes
{
    using MediaRatingProject.DB.Ratings;
    using MediaRatingProject.Enums;

    public abstract class MediaEntry
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public DateTime ReleaseYear { get; set; }
        public AgeRestriction AgeRating { get; set; }
        public List<MediaRating> Ratings { get; set; }
        public List<Favorite> FavoritedBy { get; set; }
        public double AverageRating { get; set; }

        public MediaEntry(int id,
                          string title,
                          string genre,
                          DateTime year,
                          AgeRestriction ageRestriction)
        {
            Id = id;
            Title = title;
            Genre = genre;
            ReleaseYear = year;
            AgeRating = ageRestriction;
            Ratings = new();
            FavoritedBy = new();
        }

        public override string ToString()
        {
            return $"{Id}: {Title} ({ReleaseYear.Year}), Genre: {Genre}, Age Rating: {AgeRating}";
        }
    }
}
