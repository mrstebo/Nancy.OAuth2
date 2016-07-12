using Nancy.OAuth2.Enums;
using Nancy.OAuth2.Models;
using Nancy.OAuth2.Modules;
using Nancy.OAuth2.Services;
using Nancy.Testing;
using NUnit.Framework;

namespace Nancy.OAuth2.Tests.Modules
{
    [TestFixture]
    public class TokenModuleTests
    {
        [Test]
        public void Post_With_Invalid_Grant_Type_Should_Return_404()
        {
            var browser = GetBrowser();

            var response = browser.Post("/oauth/token", with =>
            {
                with.HttpRequest();
                with.FormValue("grant_type", "bad_grant");
            });

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public void Post_With_Invalid_Grant_Type_Should_Return_ErrorResponse()
        {
            var browser = GetBrowser();

            var response = browser.Post("/oauth/token", with =>
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
            var browser = GetBrowser();

            var response = browser.Post("/oauth/token", with =>
            {
                with.HttpRequest();
                with.FormValue("grant_type", GrantTypes.Password);
            });

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void Post_With_Valid_Grant_Should_Return_TokenResponse()
        {
            var browser = GetBrowser();

            var response = browser.Post("/oauth/token", with =>
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

        private static Browser GetBrowser()
        {
            return new Browser(with =>
            {
                with.EnableAutoRegistration();
                with.Module<TokenModule>();
                with.Dependency(new TokenEndpointServiceStub());
                with.Dependency(new DefaultErrorResponseBuilder());
                with.ApplicationStartup((x, y) => OAuth.Enable());
            });
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
