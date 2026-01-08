namespace MediaRatingProject
{
    using MediaRatingProject.API;
    using MediaRatingProject.API.Services;
    using MediaRatingProject.API.Controllers;
    using MediaRatingProject.Data.Stores;
    using MediaRatingProject.Data.Ratings;

    internal class Program    
    {
        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <param name="args">Unused.</param>
        static void Main(string[] args)
        {
            string[] prefix = { "http://localhost:8080/" };
            
            // Temporary secret for JWT generation, normally this should be hidden, but for the sake of this demonstrative project it is just in Main:
            string secret = "temporary_secret_key_that_should_be_hidden_and_not_shown_publicly_like_this";          
            var jwtService = new JwtService(secret);

            // Connection string for PostgreSQL database.
            string connectionString = "Host=localhost;Port=5432;Database=mrp;Username=mrp_user;Password=mrp_pass";

            // Initialize request parser.
            var requestParser = new RequestParser();

            // Initialize data stores/repositories.
            var userStore = new UserStore(connectionString);
            var mediaStore = new MediaStore(connectionString);
            var favoriteStore = new FavoriteStore(connectionString);
            var ratingStore = new RatingStore(connectionString);

            // Manual set up of the project with dependancy injection.
            var userController = new UsersController(userStore, jwtService);
            var mediaController = new MediaController(mediaStore);  
            var favoriteController = new FavoriteController(favoriteStore);
            var ratingController = new RatingController(ratingStore);

            // Inject controllers into request handler.
            var requestHandler = new RequestHandler(userController, mediaController, favoriteController, ratingController, jwtService);  

            APIListener listener = new(prefix, requestParser, requestHandler);
            listener.Start();
        }
    }
}
