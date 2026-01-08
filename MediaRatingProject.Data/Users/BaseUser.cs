namespace MediaRatingProject.Data.Users
{
    public abstract class BaseUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int RatedMediaCount { get; set; }
        public int FavoriteMediaCount { get; set; }

        public BaseUser() { }

        public BaseUser(string name, string password)
        {
            this.Username = name;
            this.Password = password;
        }
    }
}
