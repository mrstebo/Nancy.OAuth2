using Nancy.OAuth2.Enums;
using Nancy.OAuth2.Models;
using Nancy.OAuth2.Services;
using Nancy.Testing;
using NUnit.Framework;

namespace Nancy.OAuth2.IntegrationTests.Modules
{
    [TestFixture]
    public class TokenModuleTests
    {
        private Browser _browser;

        [SetUp]
        public void SetUp()
        {
            _browser = new Browser(new Bootstrapper());

            StubFactory.Set<ITokenEndpointService, TokenEndpointServiceStub>();
        }

        [TearDown]
        public void TearDown()
        {
            StubFactory.Unset<ITokenEndpointService>();

            _browser = null;
        }

        [Test]
        public void Post_With_Invalid_Grant_Type_Should_Return_404()
        {
            var response = _browser.Post("/oauth/token", with =>
            {
                with.HttpRequest();
                with.FormValue("grant_type", "bad_grant");
            });

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public void Post_With_Invalid_Grant_Type_Should_Return_ErrorResponse()
        {
            var response = _browser.Post("/oauth/token", with =>
            {
                with.HttpRequest();
                with.FormValue("grant_type", "bad_grant");
            });
            var body = response.Body.DeserializeJson<ErrorResponse>();

            Assert.IsNotNull(body);
            Assert.AreEqual("invalid_grant", body.Error);
        }

        [Test]
        public void Post_With_Valid_Grant_Should_Return_200()
        {
            var response = _browser.Post("/oauth/token", with =>
            {
                with.HttpRequest();
                with.FormValue("grant_type", GrantTypes.Password);
            });

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void Post_With_Valid_Grant_Should_Return_TokenResponse()
        {
            var response = _browser.Post("/oauth/token", with =>
            {
                with.HttpRequest();
                with.FormValue("grant_type", GrantTypes.Password);
            });
            var body = response.Body.DeserializeJson<TokenResponse>();

            Assert.IsNotNull(body);
            Assert.AreEqual("123", body.AccessToken);
            Assert.AreEqual("bearer", body.TokenType);
            Assert.AreEqual(86400, body.ExpiresIn);
        }

        class TokenEndpointServiceStub : ITokenEndpointService
        {
            public OAuthValidationResult ValidateRequest(TokenRequest request, NancyContext context)
            {
                switch (request.GrantType)
                {
                    case GrantTypes.Password:
                        return ErrorType.None;

                    default:
                        return ErrorType.InvalidGrant;
                }
            }

            public TokenResponse CreateTokenResponse(TokenRequest request, NancyContext context)
            {
                return new TokenResponse
                {
                    AccessToken = "123",
                    TokenType = "bearer",
                    ExpiresIn = 86400
                };
            }
        }
    }
}
