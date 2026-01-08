namespace MediaRatingProject.Data.Stores
{
    public class RatingStore
    {
        private readonly string _connectionString;
        public RatingStore(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
