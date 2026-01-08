namespace MediaRatingProject.Data.Media
{
    using MediaRatingProject.Data.Ratings;

    public class BaseMedia
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } 
        public List<string> Genres { get; set; }
        public int? ReleaseYear { get; set; }
        public int? AgeRestriction { get; set; }
        public List<Rating> Ratings { get; set; }
        public int RatingCount { get; set; }
        public int FavoriteCount { get; set; }
        public double AverageRating { get; set; }
        public int? UserId { get; set; } 
        public string UserCreator { get; set; } 
        public string MediaType { get; set; }
        public BaseMedia() { }

        public BaseMedia(int id,
                          string title,
                          string description,
                          List<string> genres,
                          int year,
                          int ageRestriction)
        {
            this.Id = id;
            this.Title = title;
            this.Genres = genres;
            this.ReleaseYear = year;
            this.AgeRestriction = ageRestriction;
            this.Ratings = new();            
        }

        public override string ToString()
        {
            return $"{Id}: {Title} ({ReleaseYear}), Genre: {string.Join(", ", Genres)}, Age Rating: {AgeRestriction}";
        }
    }
}
