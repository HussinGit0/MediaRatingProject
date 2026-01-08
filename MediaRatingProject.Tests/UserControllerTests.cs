namespace MediaRatingProject.Tests
{    
    using MediaRatingProject.API.Controllers;
    using MediaRatingProject.API.DTOs;
    using MediaRatingProject.API.Interfaces;
    using MediaRatingProject.API.Requests;
    using MediaRatingProject.Data.StoreInterfaces;
    using MediaRatingProject.Data.Stores;
    using MediaRatingProject.Data.Users;
    using MediaRatingProject.Data;
    using Moq;
    using System.Text.Json;

    public class UserControllerTests
    {
        private Mock<UserStore> _mockUserStore;
        private Mock<ITokenService> _mockTokenService;
        private UsersController _controller;

        [SetUp]
        public void Setup()
        {
            _mockUserStore = new Mock<UserStore>(MockBehavior.Strict, "FakeConnectionString");
            _mockTokenService = new Mock<ITokenService>();
            _controller = new UsersController(_mockUserStore.Object, _mockTokenService.Object);
        }

        #region Register Tests

        [Test]
        public void Register_ReturnsBadRequest_WhenBodyIsEmpty()
        {
            var request = new ParsedRequestDTO { Body = "" };
            var result = _controller.Register(request);
            StringAssert.StartsWith("Invalid JSON format:", result.Message);
        }

        [Test]
        public void Register_ReturnsBadRequest_WhenUsernameOrPasswordIsMissing()
        {
            var dto = new { UserName = "", Password = "pass" };
            var request = new ParsedRequestDTO { Body = JsonSerializer.Serialize(dto) };
            var result = _controller.Register(request);
            Assert.AreEqual("Invalid username or password.", result.Message);
        }    

        [Test]
        public void Register_ReturnsBadRequest_WhenUserCreationFails()
        {
            var dto = new UserDTO { Username = "testuser", Password = "pass" };
            var request = new ParsedRequestDTO { Body = JsonSerializer.Serialize(dto) };
           

            var result = _controller.Register(request);
            StringAssert.StartsWith("Error registering user:", result.Message);
        }

        #endregion

        #region Login Tests

        [Test]
        public void Login_ReturnsInvalid_WhenUserDoesNotExist()
        {
            var dto = new UserDTO { Username = "nonexistent", Password = "pass" };
            var request = new ParsedRequestDTO { Body = JsonSerializer.Serialize(dto) };

            var userStoreMock = new Mock<IUserStore>();
            userStoreMock.Setup(s => s.CreateUser(It.IsAny<User>())).Returns(true);

            var controller = new UsersController(userStoreMock.Object, _mockTokenService.Object);

            var result = controller.Login(request);
            Assert.AreEqual("Invalid username or password.", result.Message);
        }

        [Test]
        public void Login_ReturnsInvalid_WhenPasswordIsIncorrect()
        {
            var dto = new UserDTO { Username = "testuser", Password = "wrong" };
            var request = new ParsedRequestDTO { Body = JsonSerializer.Serialize(dto) };

            var userStoreMock = new Mock<IUserStore>();
            userStoreMock.Setup(s => s.GetUserByUsername("testuser")).Returns(new User("testuser", "correct"));

            var controller = new UsersController(userStoreMock.Object, _mockTokenService.Object);

            var result = controller.Login(request);
            Assert.AreEqual("Invalid username or password.", result.Message);
        }

        #endregion

        #region GetUserByID Tests

        [Test]
        public void GetUserByID_ReturnsBadRequest_WhenUserIdMissing()
        {
            var request = new ParsedRequestDTO { Parameters = new Dictionary<string, string>() };
            var result = _controller.GetUserByID(request);
            Assert.AreEqual("Missing or invalid user ID.", result.Message);
        }

        [Test]
        public void GetUserByID_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var request = new ParsedRequestDTO { Parameters = new Dictionary<string, string> { { "userId", "1" } } };

            var userStoreMock = new Mock<IUserStore>();
            userStoreMock.Setup(s => s.GetUserByUsername("testuser")).Returns(new User("testuser", "correct"));

            var result = _controller.GetUserByID(request);
            StringAssert.StartsWith("Error fetching user:", result.Message);
        }

        #endregion
    }
}