using System.Linq;
using FakeItEasy;
using Nancy.OAuth2.Enums;
using Nancy.OAuth2.Models;
using Nancy.OAuth2.Modules;
using Nancy.OAuth2.Services;
using Nancy.Testing;
using NUnit.Framework;

namespace Nancy.OAuth2.Tests.Modules
{
    [TestFixture]
    [Parallelizable]
    public class TokenModuleTests
    {
        [SetUp]
        public void SetUp()
        {
            _tokenEndpointService = A.Fake<ITokenEndpointService>();
            _browser = new Browser(with =>
            {
                with.EnableAutoRegistration();
                with.Module<TokenModule>();
                with.Dependency(_tokenEndpointService);
                with.Dependency(new DefaultErrorResponseBuilder());
                with.ApplicationStartup((x, y) => OAuth.Enable());
            });
        }

        private ITokenEndpointService _tokenEndpointService;
        private Browser _browser;

        [Test]
        public void Post_With_Invalid_Grant_Type_Should_Return_404()
        {
            A.CallTo(() => _tokenEndpointService.ValidateRequest(A<TokenRequest>.That.Matches(x =>
                    x.GrantType == "bad_grant"), A<NancyContext>.Ignored))
                .Returns(ErrorType.InvalidGrant);

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
            A.CallTo(() => _tokenEndpointService.ValidateRequest(A<TokenRequest>.That.Matches(x =>
                    x.GrantType == "bad_grant"), A<NancyContext>.Ignored))
                .Returns(ErrorType.InvalidGrant);

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
            var tokenResponse = new TokenResponse
            {
                AccessToken = "123",
                RefreshToken = "abc",
                ExpiresIn = 86400,
                Scope = "test",
                TokenType = "bearer"
            };

            A.CallTo(() => _tokenEndpointService.CreateTokenResponse(A<TokenRequest>.Ignored, A<NancyContext>.Ignored))
                .Returns(tokenResponse);

            var response = _browser.Post("/oauth/token", with =>
            {
                with.HttpRequest();
                with.FormValue("grant_type", GrantTypes.Password);
            });
            var body = response.Body.DeserializeJson<TokenResponse>();

            Assert.IsNotNull(body);
            Assert.AreEqual("123", body.AccessToken);
            Assert.AreEqual("abc", body.RefreshToken);
            Assert.AreEqual(86400, body.ExpiresIn);
            Assert.AreEqual("test", body.Scope);
            Assert.AreEqual("bearer", body.TokenType);
        }

        [Test]
        public void Post_Should_Put_Form_Values_Into_TokenRequest()
        {
            _browser.Post("/oauth/token", with =>
            {
                with.HttpRequest();
                with.FormValue("grant_type", GrantTypes.Password);
                with.FormValue("username", "myusername");
                with.FormValue("password", "mypassword");
                with.FormValue("client_id", "my-client-id");
                with.FormValue("client_secret", "my-client-secret");
                with.FormValue("code", "123");
                with.FormValue("redirect_uri", "http://nancy-oauth.com/redirect");
                with.FormValue("scope", "my-scope-1");
                with.FormValue("scope", "my-scope-2");
            });

            A.CallTo(() => _tokenEndpointService.ValidateRequest(A<TokenRequest>.That.Matches(x =>
                    x.GrantType == GrantTypes.Password &&
                    x.Username == "myusername" &&
                    x.Password == "mypassword" &&
                    x.ClientId == "my-client-id" &&
                    x.ClientSecret == "my-client-secret" &&
                    x.Code == "123" &&
                    x.RedirectUri == "http://nancy-oauth.com/redirect" &&
                    x.Scope.All(y => y == "my-scope-1" || y == "my-scope-2")), A<NancyContext>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Post_Should_Handle_No_Scopes_On_Form()
        {
            _browser.Post("/oauth/token", with =>
            {
                with.HttpRequest();
                with.FormValue("grant_type", GrantTypes.Password);
                with.FormValue("username", "myusername");
                with.FormValue("password", "mypassword");
                with.FormValue("client_id", "my-client-id");
                with.FormValue("client_secret", "my-client-secret");
                with.FormValue("code", "123");
                with.FormValue("redirect_uri", "http://nancy-oauth.com/redirect");
            });

            A.CallTo(() => _tokenEndpointService.ValidateRequest(A<TokenRequest>.That.Matches(x =>
                    x.GrantType == GrantTypes.Password &&
                    x.Username == "myusername" &&
                    x.Password == "mypassword" &&
                    x.ClientId == "my-client-id" &&
                    x.ClientSecret == "my-client-secret" &&
                    x.Code == "123" &&
                    x.RedirectUri == "http://nancy-oauth.com/redirect" &&
                    !x.Scope.Any()), A<NancyContext>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
