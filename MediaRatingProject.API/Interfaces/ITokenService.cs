namespace MediaRatingProject.API.Interfaces
{
    public interface ITokenService
    {
        public string GenerateToken(string username, int expireMinutes = 60);
        public bool ValidateToken(string token, out string username);
    }
}
