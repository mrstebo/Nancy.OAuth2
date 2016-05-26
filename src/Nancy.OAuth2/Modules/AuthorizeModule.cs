using System;
using Nancy.ModelBinding;
using Nancy.OAuth2.Enums;
using Nancy.OAuth2.Models;
using Nancy.OAuth2.Services;
using Nancy.Security;

namespace Nancy.OAuth2.Modules
{
    public class AuthorizeModule : NancyModule
    {
        private readonly IAuthorizationEndpointService _service;
        private readonly IErrorResponseBuilder _errorResponseBuilder;

        public AuthorizeModule(
            IAuthorizationEndpointService service,
            IErrorResponseBuilder errorResponseBuilder)
            : base(OAuth.Configuration.GetFullPath(x => x.AuthorizationRequestRoute))
        {
            _service = service;
            _errorResponseBuilder = errorResponseBuilder;

            this.RequiresAuthentication();

            Get["/", ctx => OAuth.IsEnabled] = _ => Index();
            Post[OAuth.Configuration.AuthorizationAllowRoute, ctx => OAuth.IsEnabled] = _ => Allow();
            Post[OAuth.Configuration.AuthorizationDenyRoute, ctx => OAuth.IsEnabled] = _ => Deny();
        }

        private dynamic Index()
        {
            var request = this.Bind<AuthorizationRequest>();

            var result = _service.ValidateRequest(request, Context);

            if (!result.IsValid)
            {
                return Response.AsErrorResponse(BuildErrorResponse(result.ErrorType, request.State), request.RedirectUrl);
            }

            Session[Context.CurrentUser.UserName] = request;

            var authorizationView = _service.GetAuthorizationView(request, Context);

            return View[authorizationView.Item1, authorizationView.Item2];
        }

        private dynamic Allow()
        {
            var token = _service.GenerateAuthorizationToken(Context);
            var request = Session[Context.CurrentUser.UserName] as AuthorizationRequest;

            if (request == null)
            {
                return HttpStatusCode.InternalServerError;
            }

            var response = new AuthorizationResponse
            {
                Code = token,
                State = request.State
            };

            var uri = new UriBuilder(request.RedirectUrl) {Query = response.AsQueryString()};

            return Response.AsRedirect(uri.ToString());
        }

        private dynamic Deny()
        {
            var request = Session[Context.CurrentUser.UserName] as AuthorizationRequest;

            return request == null
                ? HttpStatusCode.InternalServerError
                : Response.AsErrorResponse(BuildErrorResponse(ErrorType.AccessDenied, request.State),
                    request.RedirectUrl);
        }

        private ErrorResponse BuildErrorResponse(ErrorType errorType, string state = null)
        {
            return _errorResponseBuilder.Build(errorType, state);
        }
    }
}
