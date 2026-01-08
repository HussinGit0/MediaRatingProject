namespace MediaRatingProject.API.DTOs
{
    internal class UserProfileDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int RatedMediaCount { get; set; }
        public int FavoriteMediaCount { get; set; }
    }
}
