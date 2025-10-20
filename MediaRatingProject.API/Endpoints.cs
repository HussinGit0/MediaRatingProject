namespace MediaRatingProject.API
{
    /// <summary>
    /// A class containing all the paths for the API endpoints as specified in the project requirements.
    /// These endpoints have been testing using the POSTMAN collection provided in the course in Moodle:
    /// https://moodle.technikum-wien.at/pluginfile.php/2671725/mod_resource/content/1/MRP_Postman_Collection.json
    /// </summary>
    public static class EndPoints
    {        
        public const string USERS_REGISTER_REQUEST = "/api/users/register"; // Post
        public const string USERS_LOGIN_REQUEST = "/api/users/login"; // Post
        public const string USERS_PROFILE_REQUEST = "/api/users/{userId}/profile"; // Get Put
        public const string USERS_RATINGS_REQUEST = "/api/users/{userId}/ratings"; // Get
        public const string USERS_FAVORITES_REQUEST = "/api/users/{userId}/favorites"; // Get 
        public const string USERS_RECOMMENDATION_REQUEST = "/api/users/{userId}/recommendations"; // Get 
        public const string MEDIA_REQUEST = "/api/media"; // Get Post
        public const string MEDIA_ID_REQUEST = "/api/media/{mediaId}"; // Delete, get, put
        public const string MEDIA_RATE_REQUEST = "/api/media/{mediaId}/rate"; // Post 
        public const string MEDIA_LIKE_REQUEST = "/api/ratings/{ratingId}/like"; // Post
        public const string MEDIA_FAVORITE_REQUEST = "/api/media/{mediaId}/favorite"; // Post delete
        public const string LEADERBOARD_REQUEST = "/api/leaderboard"; // Get
        public const string RATINGS_ID_REQUEST = "/api/ratings/{ratingId}"; // Put delete
        public const string RATINGS_ID_CONFIRM_REQUEST = "/api/ratings/{ratingId}/confirm"; // Post 
    }
}
