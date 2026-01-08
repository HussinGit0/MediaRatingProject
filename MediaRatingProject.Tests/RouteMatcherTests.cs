namespace MediaRatingProject.Tests
{
    using NUnit.Framework;
    using MediaRatingProject.API;
    using System.Collections.Generic;
    public class RouteMatcherTests
    {
        [Test]
        public void TryMatch_ReturnsTrue_ForExactStaticRoute()
        {
            var template = "/api/media";
            var request = "/api/media";

            var result = RouteMatcher.TryMatch(template, request, out var parameters);

            Assert.IsTrue(result);
        }

        [Test]
        public void TryMatch_ReturnsTrue_ForRouteWithSingleParameter()
        {
            var template = "/api/users/{userId}";
            var request = "/api/users/42";

            var result = RouteMatcher.TryMatch(template, request, out var parameters);

            Assert.IsTrue(result);
            Assert.AreEqual("42", parameters["userId"]);
        }

        [Test]
        public void TryMatch_ReturnsTrue_ForRouteWithMultipleParameters()
        {
            var template = "/api/users/{userId}/ratings/{ratingId}";
            var request = "/api/users/10/ratings/99";

            var result = RouteMatcher.TryMatch(template, request, out var parameters);

            Assert.IsTrue(result);
            Assert.AreEqual("10", parameters["userId"]);
            Assert.AreEqual("99", parameters["ratingId"]);
        }

        [Test]
        public void TryMatch_AllowsNonNumericParameterValues()
        {
            var template = "/api/media/{text}";
            var request = "/api/media/inception-2010";

            var result = RouteMatcher.TryMatch(template, request, out var parameters);

            Assert.IsTrue(result);
            Assert.AreEqual("inception-2010", parameters["text"]);
        }

        [Test]
        public void TryMatch_ReturnsFalse_WhenRouteDoesNotMatch()
        {
            var template = "/api/users/{userId}";
            var request = "/api/media/5";

            var result = RouteMatcher.TryMatch(template, request, out var parameters);

            Assert.IsFalse(result);
            Assert.IsEmpty(parameters);
        }

        [Test]
        public void TryMatch_ReturnsFalse_WhenSegmentCountDiffers()
        {
            var template = "/api/users/{userId}";
            var request = "/api/users/5/ratings";

            var result = RouteMatcher.TryMatch(template, request, out var parameters);

            Assert.IsFalse(result);
            Assert.IsEmpty(parameters);
        }

        [Test]
        public void TryMatch_ReturnsFalse_WhenTemplateIsEmpty()
        {
            var template = "";
            var request = "/api/media";

            var result = RouteMatcher.TryMatch(template, request, out var parameters);

            Assert.IsFalse(result);
        }

        [Test]
        public void TryMatch_ReturnsFalse_WhenRequestIsEmpty()
        {
            var template = "/api/media";
            var request = "";

            var result = RouteMatcher.TryMatch(template, request, out var parameters);

            Assert.IsFalse(result);
        }
    }
}
