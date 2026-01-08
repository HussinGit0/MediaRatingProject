namespace MediaRatingProject
{
    using MediaRatingProject.API;
    using MediaRatingProject.API.Controllers;
    using MediaRatingProject.API.Services;
    using MediaRatingProject.Data.Stores;

    /// <summary>
    /// Class representing the entry point of the application, responsible for initializing and running the API server.
    /// </summary>
    public class Application
    {        
        private APIListener _listener;

        public void SetUp()
        {
            string[] prefix = { "http://localhost:8080/" };

            // Temporary secret for JWT generation, normally this should be hidden, but for the sake of this demonstrative project it is just in Main:
            string secret = "temporary_secret_key_that_should_be_hidden_and_not_shown_publicly_like_this";
            var jwtService = new JwtService(secret);

            // Connection string for PostgreSQL database.
            string connectionString = "Host=localhost;Port=5432;Database=mrp;Username=mrp_user;Password=mrp_pass";

            // Initialize request parser.
            var requestParser = new RequestParser();

            // Initialize data stores/repositories responsible for communicating with the database.
            var userStore = new UserStore(connectionString);
            var mediaStore = new MediaStore(connectionString);
            var favoriteStore = new FavoriteStore(connectionString);
            var ratingStore = new RatingStore(connectionString);

            // Manual set up of the project's controllers with dependancy injection. The four following classes are responsible for handling the business logic.
            var userController = new UsersController(userStore, jwtService);
            var mediaController = new MediaController(mediaStore);
            var favoriteController = new FavoriteController(favoriteStore);
            var ratingController = new RatingController(ratingStore);

            // Inject controllers into request handler.
            var requestHandler = new RequestHandler(userController, mediaController, favoriteController, ratingController, jwtService);

            _listener = new(prefix, requestParser, requestHandler);
        }
        /// <summary>
        /// Application starting point
        /// </summary>
        public void Run()
        {
            _listener.Start();
        }
    }
}
