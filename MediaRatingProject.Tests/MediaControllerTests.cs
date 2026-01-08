namespace MediaRatingProject.Tests
{
    using MediaRatingProject.API.Controllers;
    using MediaRatingProject.API.Requests;
    using MediaRatingProject.Data.Media;
    using MediaRatingProject.Data.Ratings;
    using MediaRatingProject.Data.StoreInterfaces;
    using Moq;
    using System.Text.Json;

    public class MediaControllerTests
    {
        private Mock<IMediaStore> _mediaStoreMock;
        private MediaController _controller;

        [SetUp]
        public void Setup()
        {
            _mediaStoreMock = new Mock<IMediaStore>();
            _controller = new MediaController(_mediaStoreMock.Object);
        }

        [Test]
        public void GetLeaderboard_ReturnsOk_WhenStoreReturnsData()
        {
            _mediaStoreMock.Setup(s => s.GetLeaderboard()).Returns(new List<MediaSummaryDTO>());

            var result = _controller.GetLeaderboard();

            Assert.AreEqual("Fetched all media successfully.", result.Message);
        }

        [Test]
        public void GetLeaderboard_ReturnsBadRequest_OnException()
        {
            _mediaStoreMock.Setup(s => s.GetLeaderboard()).Throws(new System.Exception("DB error"));

            var result = _controller.GetLeaderboard();

            StringAssert.StartsWith("Error fetching media:", result.Message);
        }


        [Test]
        public void GetMediaById_ReturnsNotFound_WhenMediaDoesNotExist()
        {
            var request = new ParsedRequestDTO
            {
                Parameters = new Dictionary<string, string> { { "mediaId", "1" } }
            };

            _mediaStoreMock.Setup(s => s.GetMediaById(1)).Returns((BaseMedia)null);

            var result = _controller.GetMediaById(request);

            Assert.AreEqual("Media with '1' not found.", result.Message);
        }

        [Test]
        public void CreateMedia_ReturnsOk_WhenMediaIsValid()
        {
            var body = JsonSerializer.Serialize(new
            {
                mediaType = "movie",
                title = "Inception",
                description = "Some description",
                genres = new[] { "Sci-Fi", "Thriller" },
                releaseYear = 2010,
                ageRestriction = 12
            });

            var request = new ParsedRequestDTO
            {
                Body = body,
                UserID = 1,
                UserName = "test"
            };

            _mediaStoreMock.Setup(s => s.CreateMedia(It.IsAny<BaseMedia>())).Returns(true);

            var result = _controller.CreateMedia(request);

            Assert.AreEqual("Media created successfully.", result.Message);
        }

        [Test]
        public void CreateMedia_ReturnsBadRequest_ForUnknownMediaType()
        {
            var body = JsonSerializer.Serialize(new { mediaType = "unknown" });
            var request = new ParsedRequestDTO { Body = body };

            var result = _controller.CreateMedia(request);

            StringAssert.Contains("Unknown mediaType", result.Message);
        }

        [Test]
        public void UpdateMedia_ReturnsUnauthorized_WhenUserIsNotOwner()
        {
            var existing = new MovieMedia { Id = 1, UserId = 99 };

            _mediaStoreMock.Setup(s => s.GetMediaById(1)).Returns(existing);

            var request = new ParsedRequestDTO
            {
                Parameters = new Dictionary<string, string> { { "mediaId", "1" } },
                UserID = 1
            };

            var result = _controller.UpdateMedia(request);

            Assert.AreEqual("You are unauthorized to update this media.", result.Message);
        }

        [Test]
        public void UpdateMedia_ReturnsOk_WhenValid()
        {
            var existing = new BaseMedia
            {
                Id = 1,
                UserId = 1,
                Title = "ToChange"
            };

            _mediaStoreMock.Setup(s => s.GetMediaById(1)).Returns(existing);
            _mediaStoreMock.Setup(s => s.UpdateMedia(It.IsAny<BaseMedia>(), 1)).Returns(true);

            var body = JsonSerializer.Serialize(new
            {
                mediaType = "movie",
                title = "Updated",
                description = "Updated",
                genres = new[] { "Drama" },
                releaseYear = 2020,
                ageRestriction = 16
            });

            var request = new ParsedRequestDTO
            {
                Parameters = new Dictionary<string, string> { { "mediaId", "1" } },
                Body = body,
                UserID = 1
            };

            var result = _controller.UpdateMedia(request);

            Assert.AreEqual("Media updated successfully.", result.Message);
        }

        [Test]
        public void DeleteMedia_ReturnsOk_WhenDeleted()
        {
            var media = new MovieMedia { Id = 1, UserId = 1 };

            _mediaStoreMock.Setup(s => s.GetMediaById(1)).Returns(media);
            _mediaStoreMock.Setup(s => s.DeleteMedia(1)).Returns(true);

            var request = new ParsedRequestDTO
            {
                Parameters = new Dictionary<string, string> { { "mediaId", "1" } },
                UserID = 1
            };

            var result = _controller.DeleteMedia(request);

            StringAssert.StartsWith("Media", result.Message);
        }
    }
}
