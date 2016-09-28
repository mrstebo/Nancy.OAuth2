using System;
using Nancy.OAuth2.Models;
using Nancy.OAuth2.Services;
using NUnit.Framework;

namespace Nancy.OAuth2.Tests.Services
{
    [TestFixture]
    [Parallelizable]
    public class DefaultAuthorizationEndpointServiceTests
    {
        private IAuthorizationEndpointService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new DefaultAuthorizationEndpointService();
        }

        [Test]
        public void GenerateAuthorizationToken_ShouldThrow_NotImplementedException()
        {
            var request = new AuthorizationRequest();
            var context = new NancyContext();

            Assert.Throws<NotImplementedException>(() => _service.GenerateAuthorizationToken(request, context));
        }

        [Test]
        public void ValidateRequest_ShouldThrow_NotImplementedException()
        {
            var request = new AuthorizationRequest();
            var context = new NancyContext();

            Assert.Throws<NotImplementedException>(() => _service.ValidateRequest(request, context));
        }

        [Test]
        public void GetAuthorizationView_ShouldThrow_NotImplementedException()
        {
            var request = new AuthorizationRequest();
            var context = new NancyContext();

            Assert.Throws<NotImplementedException>(() => _service.GetAuthorizationView(request, context));
        }
    }
}
