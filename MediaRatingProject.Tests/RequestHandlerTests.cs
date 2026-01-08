using MediaRatingProject.API;
using MediaRatingProject.API.Interfaces;
using MediaRatingProject.API.Requests;
using Moq;

namespace MediaRatingProject.Tests
{
    public class RequestHandlerTests
    {
        private Mock<IUsersController> _usersController;
        private Mock<IMediaController> _mediaController;
        private Mock<IFavoriteController> _favoriteController;
        private Mock<IRatingController> _ratingController;
        private Mock<ITokenService> _tokenService;

        private RequestHandler _handler;

        [SetUp]
        public void Setup()
        {
            _usersController = new Mock<IUsersController>();
            _mediaController = new Mock<IMediaController>();
            _favoriteController = new Mock<IFavoriteController>();
            _ratingController = new Mock<IRatingController>();
            _tokenService = new Mock<ITokenService>();

            _handler = new RequestHandler(
                _usersController.Object,
                _mediaController.Object,
                _favoriteController.Object,
                _ratingController.Object,
                _tokenService.Object);
        }

        [Test]
        public void HandleRequest_ReturnsBadRequest_WhenRequestIsInvalid()
        {
            var request = new ParsedRequestDTO { IsSuccessful = false };

            var result = _handler.HandleRequest(request);

            Assert.AreEqual("Bad request! Could not parse.", result.Message);
        }


        [Test]
        public void HandleRequest_ReturnsUnauthorized_WhenTokenInvalid()
        {
            var request = new ParsedRequestDTO
            {
                IsSuccessful = true,
                HttpMethod = "GET",
                Path = EndPoints.USERS_PROFILE_REQUEST,
                Token = "Whatever"
            };

            _tokenService.Setup(t => t.ValidateToken("Whatever", out It.Ref<string>.IsAny)).Returns(false);

            var result = _handler.HandleRequest(request);

            Assert.AreEqual("Unauthorized or missing token.", result.Message);
        }
    }
}
