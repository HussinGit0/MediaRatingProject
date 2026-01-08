# Media Rating Project (MRP)

MRP is a simple media rating platform API which allows a user to register and log in. Create media entries for different types of media. Rate them, favorite them, and comment on them.

Most functionalities have been implemented, with the exception of a few:
- Recommendation based on user's interests.
- User leaderboard (which is replaced by a media leaderboard based on the average rating)


## Table of Contents

- [Project Overview](#project-overview)
- [Usage](#usage)
- [Architecture and Technical Decisions](#architecture-and-technical-decisions)
- [GIT Link](#git-link)
- [Setup](#setup)
- [Mermaid Code](#mermaid-code)
- [Lessons Learned](#lessons-learned)
- [Solid Principles](#solid-principles)
- [Unit Testing Strategy](#unit-testing-strategy)

## Projet Overview

The project is built with 3 Layers:
- The first is the server executable (Program.cs), which handles the API listener's initialization and dependancy injection.
- The second is the API library, with its entry point APIListener.cs. It consists of a listener, a request parser, and a request handler. This Layer also contains controllers for the business logic and various DTOs.
- The third layer is the Data layer, which will later be replaced with DB calls. For the intermediate handout, it consists of various model classes at the moment.

The Endpoints were implemented based on the provided Postman collection in with little to no changes: https://moodle.technikum-wien.at/pluginfile.php/2671725/mod_resource/content/1/MRP_Postman_Collection.json

Testing the end points was also done in POSTMAN.

## Usage
Since designing a front-end was not part of the project, the application can be used using POSTMAN or programs that allow for sending HTTP Requests. In order to use the application, run the following command to start docker:

docker compose up -d 

Then run

Get-Content schema.sql | docker exec -i mrp-postgres psql -U mrp_user -d mrp


inside the folder with the sql schema to create the database.

In order to use all the features, one must send a Register POST request with a username and password, then send a LOGIN request to retrieve an authentication token. This token is tied to the current user this session, and all changes will be tracked to them. This means that if another user tried to edit or delete entries or ratings made by other users, they will not be able to.

## Architecture
The Project attempts to separate concerns and follow the SOLID principles. For example, tasks and functionalities are reserved to their own specific classes. Managing users is only done via the UsersController.cs, same with managing media using MediaController.cs, both having an aggregate relation with UserStore.cs and MediaStore.cs from MediaRatingProject.Data respectively. Parsing the request and its execution are also done in separate classe (RequestParser.cs, RequestHandler.cs), this allows for better debugging, because if a request is faulty, it will be easier to trace why that is the case. The following is an image depecting the class diagram, which is recreated in Mermaidcharts (https://mermaid.js.org/). For the full mermaid code, see the Mermaid Code section at the end:

![UML Diagram](diagram.png)


There are some optional features planned for the full project, such as using the AgeRestriction.cs enums to categorize the age ratings into different age brackets (Everyone, Teen, Mature, Unrated). As well as using the DateTime class for the release Year instead of just an integer. This will however require making changes to the defined endpoints (from integer fields to a specific data type).


## GIT Link
The project can be found here:
https://github.com/HussinGit0/MediaRatingProject

## Setup
To test the project, simply run the console application (MediaRatingProject\program.cs). This will activate the listener.
To test the variou end points, one could use the previously linked POSTMAN collection and ensure that all endpoints are active. As well as test individual requests.

## Mermaid Code

classDiagram
    class BaseUser {
        +int Id
        +string Username
        +string Password
        +List<Rating> RatedMedia
        +List<Favorite> FavoriteMedia
    }

    class User {
    }

    class BaseMedia {
        +int Id
        +string Title
        +string Description
        +string[] Genres
        +int ReleaseYear
        +int AgeRestriction
        +List<Rating> Ratings
        +List<Favorite> FavoritedBy
        +float AverageRating
    }

    class MovieMedia {
    }

    class SeriesMedia {
    }

    class GameMedia {
    }

    class Favorite {
        +BaseUser User
        +BaseMedia Media
    }

    class Rating {        
        +BaseUser User
        +BaseMedia Media
        +int Score
        +bool Approved
        +string Comment
        +Dictionary<BaseUser, bool> Likes
    }

    class UserDTO {
        +string Username
        +string Password
    }

    class ParsedRequestDTO {
        +bool IsSuccessful
        +string HttpMethod
        +string Path
        +string Body
        +string Token
    }

    class UserStore {
        -int _idCount
        -Dictionary<int, BaseUser> _userStore
        +bool AddUser(BaseUser user)
        +bool RemoveUser(int userId)
        +bool UpdateUser(BaseUser updatedUser, int id)
        +BaseUser GetUserById(int id)
        +BaseUser GetUserByUsername(string username)
    }

    class MediaStore {
        -int _idCount
        -Dictionary<int, BaseMedia> _mediaStore
        +bool AddMedia(BaseMedia media)
        +BaseMedia GetMediaById(int id)
        +bool RemoveMedia(int mediaId)
        +bool UpdateMedia(BaseMedia updatedMedia, int id)
        +IReadOnlyCollection<BaseMedia> GetAllMedia()
    }

    class UsersController {
        -UserStore _userStore
        -ITokenService _jwtService
        +ResponseHandler Register(ParsedRequestDTO request)
        +ResponseHandler Login(ParsedRequestDTO request)
    }

    class MediaController {
        -MediaStore _mediaStore
        +ResponseHandler GetAllMedia()
        +ResponseHandler GetMediaById(ParsedRequestDTO request)
        +ResponseHandler GetMediaById(ParsedRequestDTO request)
        +public ResponseHandler CreateMedia(ParsedRequestDTO request)
        +public ResponseHandler UpdateMedia(ParsedRequestDTO request)
        +public ResponseHandler DeleteMedia(ParsedRequestDTO request)
    }

    class ITokenService {
        +GenerateToken(string username, expireMinutes)
        +bool ValidateToken(string token, out string username)
    }

    class JWTService {
        +Implements ITokenService
    }

    class APIListener {
        -HttpListner _listener
        -RequestParser _requestParser
        -requestHandler _requestHandler
        +void Start()
    }

    class RouteMatcher {
        +bool TryMatch(string routeTemplate, string requestedRoute, out Dictionary<string, string> parameters)
    }

    class RequestParser {
        +ParsedRequestDTO ParseRequest(HttpListenerRequest request, string body)
        -ParsedRequestDTO ParsePOSTRequest(HttpListenerRequest request, string body)
        -ParsedRequestDTO ParseGETRequest(HttpListenerRequest request, string body)
        -ParsedRequestDTO ParsePUTRequest(HttpListenerRequest request, string body)
        -ParsedRequestDTO ParseDELETERequest(HttpListenerRequest request, string body)
    }

    class RequestHandler {
        -UsersController _usersController
        -MediaController _mediaController
        +ResponseHandler HandleRequest(ParsedRequestDTO request)
        -ResponseHandler HandlePOSTRequest(ParsedRequestDTO request)
        -ResponseHandler HandleGETRequest(ParsedRequestDTO request)
        -ResponseHandler HandlePUTRequest(ParsedRequestDTO request)
        -ResponseHandler HandleDELETERequest(ParsedRequestDTO request)
    }

    class ResponseHandler {
        +int StatusCode
        +string Message
        +string Body
    }

    class EndPoints {
        CONTAINS CONSTANTS FOR ENDPOINT PATHS
    }

    %% Inheritance
    User --|> BaseUser
    MovieMedia --|> BaseMedia
    SeriesMedia --|> BaseMedia
    GameMedia --|> BaseMedia
    JWTService --|> ITokenService

    %% Associations
    BaseUser --> Rating
    BaseMedia --> Rating
    BaseUser --> Favorite
    BaseMedia --> Favorite
    Favorite --> BaseUser
    Favorite --> BaseMedia

    %% Stores / composition
    UserStore o-- User
    MediaStore o-- BaseMedia

    %% Controllers / services
    UsersController ..> UserStore
    UsersController ..> ITokenService
    MediaController ..> MediaStore
    JWTService ..> ITokenService

    %% API pipeline
    APIListener --> RouteMatcher
    RouteMatcher --> RequestParser
    RequestParser --> RequestHandler
    RequestHandler --> UsersController
    RequestHandler --> MediaController
    RequestHandler --> ResponseHandler
    RouteMatcher --> EndPoints

## Lessons Learned
The biggest issue with the project was the lack of time management leading to suddenly having to crunch too much in a short span of time, potentailly overlooking some problems. Next time, the project will be started on time.

## Solid Principles
The application seperates concerns and implements the single responsibility principle into different classes, so that each class is responsible for one thing. For example, the process of handling requests takes place in sequential steps indifferent classes:
Listen -> Recieve request -> parse request (RequestParser.cs) -> handle request (RequestHandler.cs) -> Send request to respective controller (User/Media/Ratings/FavoriteController.cs) -> Send relavent information to the store repository (User/Media/Ratings/FavoriteStore.cs) -> Return a response (ResponseHandler.cs).

There are separate Controller classes for each type of data: Users, Media, Ratings, Favorites. The controllers are only responsible for extracting the relavent information from the request, and sending them over to the respective store/repository, which communicate directly with the database.

The application also depends on interfaces which can be easily swapped with other ones, which is useful in cases where the implementation or logic need to change, but the outcome/inputs remain the same (e.g. migrating to a different database). Further more, new functionality and end points can be added without altering old code.

Using dependancy injection allows for easier testing and mocking.

## Unit Testing Strategy
For unit testing the idea was to test at least a positive case and a negative case for all public functions in the business logic. Ideally, each route would also be tested to ensure that it is possible to reach. However, due to bad time constraint, a few key functions were tested only.