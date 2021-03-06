﻿using System;
using System.Collections.Generic;
using Nancy.OAuth2.Enums;
using Nancy.OAuth2.Models;
using Nancy.OAuth2.Modules;
using Nancy.OAuth2.Services;
using Nancy.Security;
using Nancy.Testing;
using NUnit.Framework;

namespace Nancy.OAuth2.Tests.Modules
{
    [TestFixture]
    public class AuthorizeModuleTests
    {
        [Test]
        public void Get_When_Not_Logged_In_Should_Return_401()
        {
            var browser = GetBrowser();

            var response = browser.Get("/oauth/authorize", with =>
            {
                with.HttpRequest();
                with.Query("response_type", "code");
                with.Query("client_id", "123");
                with.Query("redirect_url", "http://localhost/redirect");
                with.Query("state", "123");
            });

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public void Get_When_Logged_In_When_Validation_Fails_Should_Redirect_To_RedirectUrl_With_Error_In_Query_String()
        {
            var browser = GetAuthenticatedBrowser("testuser");

            var response = browser.Get("/oauth/authorize", with =>
            {
                with.HttpRequest();
                with.Query("response_type", "code");
                with.Query("client_id", "bad");
                with.Query("redirect_uri", "http://localhost/callback");
                with.Query("state", "123");
            });

            response.ShouldHaveRedirectedTo("http://localhost/callback?error=invalid_request&errordescription=The+request+is+missing+a+required+parameter%2c+includes+an+unsupported+parameter+or+parameter+value%2c+or+is+otherwise+malformed.&state=123");
        }

        [Test]
        public void Get_When_Logged_In_Should_Show_Authorize_View()
        {
            var browser = GetAuthenticatedBrowser("testuser");

            var response = browser.Get("/oauth/authorize", with =>
            {
                with.HttpRequest();
                with.Query("response_type", "code");
                with.Query("client_id", "123");
                with.Query("redirect_url", "http://localhost/redirect");
                with.Query("state", "123");
            });

            Assert.AreEqual("Authorize", response.Context.NegotiationContext.ViewName);
        }

        [Test]
        public void Post_Allow_When_Not_Logged_In_Should_Return_401()
        {
            var browser = GetBrowser();

            var response = browser.Post("/oauth/authorize/allow", with => with.HttpRequest());

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public void Post_Allow_When_No_Request_In_Session_Should_Return_500()
        {
            var browser = GetAuthenticatedBrowser("testuser");

            var response = browser.Post("/oauth/authorize/allow", with => with.HttpRequest());

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Test]
        public void Post_Allow_Should_Redirect_To_RedirectUrl_With_AuthorizationResponse_In_Query_String()
        {
            var browser = GetAuthenticatedBrowser("testuser", new Dictionary<string, object>
            {
                {
                    "testuser", new AuthorizationRequest
                    {
                        RedirectUri = "http://localhost/callback",
                        State = "123"
                    }
                }
            });

            var response = browser.Post("/oauth/authorize/allow", with => with.HttpRequest());

            response.ShouldHaveRedirectedTo("http://localhost:80/callback?code=12345678&state=123");
        }

        [Test]
        public void Post_Deny_When_Not_Logged_In_Should_Return_401()
        {
            var browser = GetBrowser();

            var response = browser.Post("/oauth/authorize/deny", with => with.HttpRequest());

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public void Post_Deny_When_No_Request_In_Session_Should_Return_500()
        {
            var browser = GetAuthenticatedBrowser("testuser");

            var response = browser.Post("/oauth/authorize/deny", with => with.HttpRequest());

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Test]
        public void Post_Deny_Should_Redirect_To_RedirectUrl_With_ErrorResponse_In_Query_String()
        {
            var browser = GetAuthenticatedBrowser("testuser", new Dictionary<string, object>
            {
                {
                    "testuser", new AuthorizationRequest
                    {
                        RedirectUri = "http://localhost/callback",
                        State = "123"
                    }
                }
            });

            var response = browser.Post("/oauth/authorize/deny", with => with.HttpRequest());

            response.ShouldHaveRedirectedTo("http://localhost/callback?error=access_denied&errordescription=The+user+denied+your+request.&state=123");
        }
        
        private static Browser GetBrowser()
        {
            return new Browser(with =>
            {
                with.EnableAutoRegistration();
                with.Module<AuthorizeModule>();
                with.Dependency(new AuthorizationEndpointServiceStub());
                with.Dependency(new DefaultErrorResponseBuilder());
                with.RootPathProvider<TestRootPathProvider>();
                with.ApplicationStartup((x, y) => OAuth.Enable());
                with.RequestStartup((container, pipelines, context) =>
                {
                    pipelines.WithSession(new Dictionary<string, object>());
                });
            });
        }

        private static Browser GetAuthenticatedBrowser(string username, IDictionary<string, object> session = null)
        {
            return new Browser(with =>
            {
                with.EnableAutoRegistration();
                with.Module<AuthorizeModule>();
                with.Dependency(new AuthorizationEndpointServiceStub());
                with.Dependency(new DefaultErrorResponseBuilder());
                with.RootPathProvider<TestRootPathProvider>();
                with.ApplicationStartup((x, y) => OAuth.Enable());
                with.RequestStartup((container, pipelines, context) =>
                {
                    context.CurrentUser = new TestUserIdentity {UserName = username};
                    pipelines.WithSession(session ?? new Dictionary<string, object>());
                });
            });
        }

        class TestUserIdentity : IUserIdentity
        {
            public string UserName { get; set; }
            public IEnumerable<string> Claims { get; set; }
        }

        class AuthorizationEndpointServiceStub : IAuthorizationEndpointService
        {
            public string GenerateAuthorizationToken(AuthorizationRequest request, NancyContext context)
            {
                return "12345678";
            }

            public OAuthValidationResult ValidateRequest(AuthorizationRequest request, NancyContext context)
            {
                if (request.ClientId == "123")
                    return ErrorType.None;
                return ErrorType.InvalidRequest;
            }

            public Tuple<string, object> GetAuthorizationView(AuthorizationRequest request, NancyContext context)
            {
                return new Tuple<string, object>("Authorize", request);
            }
        }
    }
}
