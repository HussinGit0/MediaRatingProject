namespace MediaRatingProject.Data.Users
{
    using MediaRatingProject.Data.Ratings;

    public abstract class BaseUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Rating> RatedMedia { get; set; }
        public List<Favorite> FavoriteMedia { get; set; }

        public BaseUser() { }

        public BaseUser(string name, string password)
        {
            this.Username = name;
            this.Password = password;
        }
    }
}
