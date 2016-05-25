using Nancy.ModelBinding;
using Nancy.OAuth2.Enums;
using Nancy.OAuth2.Models;
using Nancy.OAuth2.Services;

namespace Nancy.OAuth2.Modules
{
    public class TokenModule : NancyModule
    {
        private readonly ITokenEndpointService _service;
        private readonly IErrorResponseBuilder _errorResponseBuilder;

        public TokenModule(
            ITokenEndpointService service,
            IErrorResponseBuilder errorResponseBuilder)
            : base(OAuth.Configuration.GetFullPath(x => x.TokenRoute))
        {
            _service = service;
            _errorResponseBuilder = errorResponseBuilder;

            Post["/"] = _ => CreateToken();
        }

        private dynamic CreateToken()
        {
            var request = this.Bind<TokenRequest>();
            
            var result = _service.ValidateRequest(request, Context);

            if (!result.IsValid)
            {
                return Response.AsErrorResponse(BuildErrorResponse(result.ErrorType), request.RedirectUri);
            }

            var response = _service.CreateTokenResponse(request, Context);

            return Response.AsJson(response);
        }

        private ErrorResponse BuildErrorResponse(ErrorType errorType, string state = null)
        {
            return _errorResponseBuilder.Build(errorType, state);
        }
    }
}
