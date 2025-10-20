namespace MediaRatingProject
{
    using MediaRatingProject.API;
    using MediaRatingProject.API.Services;
    using MediaRatingProject.API.Controllers;
    using MediaRatingProject.Data.Stores;

    internal class Program    
    {
        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <param name="args">Unused.</param>
        static void Main(string[] args)
        {
            string[] prefix = { "http://localhost:8080/" };
            
            // Temporary secret for JWT generation:
            string secret = "temporary_secret_key_that_should_be_hidden_and_not_shown_publicly_like_this";
            var jwtService = new JwtService(secret);

            // Manual set up of the project with dependancy injection.
            var userController = new UsersController(new UserStore(), jwtService);
            var mediaController = new MediaController(new MediaStore());            
            var requestParser = new RequestParser();
            var requestHandler = new RequestHandler(userController, mediaController);  

            APIListener listener = new(prefix, requestParser, requestHandler);
            listener.Start();
        }
    }
}
