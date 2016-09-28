using System;
using Nancy.OAuth2.Models;
using Nancy.OAuth2.Services;
using NUnit.Framework;

namespace Nancy.OAuth2.Tests.Services
{
    [TestFixture]
    [Parallelizable]
    public class DefaultTokenEndpointServiceTests
    {
        private ITokenEndpointService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new DefaultTokenEndpointService();
        }

        [Test]
        public void ValidateRequest_ShouldThrow_NotImplementedException()
        {
            var request = new TokenRequest();
            var context = new NancyContext();

            Assert.Throws<NotImplementedException>(() => _service.ValidateRequest(request, context));
        }

        [Test]
        public void CreateTokenResponse_ShouldThrow_NotImplementedException()
        {
            var request = new TokenRequest();
            var context = new NancyContext();

            Assert.Throws<NotImplementedException>(() => _service.CreateTokenResponse(request, context));
        }
    }
}
