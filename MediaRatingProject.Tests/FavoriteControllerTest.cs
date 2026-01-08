using MediaRatingProject.API.Controllers;
using MediaRatingProject.API.Requests;
using MediaRatingProject.Data.Media;
using MediaRatingProject.Data.StoreInterfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingProject.Tests
{
    internal class FavoriteControllerTest
    {
        private Mock<IFavoriteStore> _favoriteStoreMock;
        private FavoriteController _controller;

        [SetUp]
        public void Setup()
        {
            _favoriteStoreMock = new Mock<IFavoriteStore>();
            _controller = new FavoriteController(_favoriteStoreMock.Object);
        }

        [Test]
        public void GetFavoritesByUserID_ReturnsBadRequest_WhenUserIdIsNull()
        {
            var request = new ParsedRequestDTO
            {
                UserID = null
            };

            var result = _controller.GetFavoritesByUserID(request);
            Assert.AreEqual("Invalid or missing user ID.", result.Message);
        }

        [Test]
        public void GetFavoritesByUserID_ReturnsOk_WhenFavoritesExist()
        {
            var request = new ParsedRequestDTO
            {
                UserID = 1
            };

            _favoriteStoreMock.Setup(s => s.GetFavoritesByUserID(1)).Returns(new List<BaseMedia>());

            var result = _controller.GetFavoritesByUserID(request);

            Assert.AreEqual("Fetched favorites successfully.", result.Message);
        }

        [Test]
        public void MarkFavorite_ReturnsOk_WhenSuccessfullyMarked()
        {
            var request = new ParsedRequestDTO
            {
                UserID = 1,
                Parameters = new Dictionary<string, string>
                {
                    { "mediaId", "10" }
                }
            };

            _favoriteStoreMock.Setup(s => s.MarkFavorite(1, 10)).Returns(true);

            var result = _controller.MarkFavorite(request);
            Assert.AreEqual("Media marked as favorite.", result.Message);
        }

        [Test]
        public void MarkFavorite_ReturnsBadRequest_WhenAlreadyFavorite()
        {
            var request = new ParsedRequestDTO
            {
                UserID = 1,
                Parameters = new Dictionary<string, string>
                {
                    { "mediaId", "10" }
                }
            };

            _favoriteStoreMock.Setup(s => s.MarkFavorite(1, 10)).Returns(false);

            var result = _controller.MarkFavorite(request);
            Assert.AreEqual("Media already marked as favorite or could not be added.", result.Message);
        }        
    }
}
