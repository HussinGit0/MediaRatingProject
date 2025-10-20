namespace MediaRatingProject.Data.Media
{
    using MediaRatingProject.Data.Enums;
    using MediaRatingProject.Data.Ratings;

    public abstract class BaseMedia
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } 
        public string[] Genres { get; set; }
        public DateTime ReleaseYear { get; set; }
        public int AgeRating { get; set; }
        public List<Rating> Ratings { get; set; }
        public List<Favorite> FavoritedBy { get; set; }
        public float AverageRating { get; set; }

        public BaseMedia(int id,
                          string title,
                          string description,
                          string[] genres,
                          DateTime year,
                          int ageRestriction)
        {
            this.Id = id;
            this.Title = title;
            this.Genres = genres;
            this.ReleaseYear = year;
            this.AgeRating = ageRestriction;
            this.Ratings = new();
            this.FavoritedBy = new();
        }

        public override string ToString()
        {
            return $"{Id}: {Title} ({ReleaseYear.Year}), Genre: {string.Join(", ", Genres)}, Age Rating: {AgeRating}";
        }
    }
}
