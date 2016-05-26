using System;
using Nancy.OAuth2.Models;

namespace Nancy.OAuth2.Services
{
    public interface IAuthorizationEndpointService
    {
        string GenerateAuthorizationToken(NancyContext context);
        OAuthValidationResult ValidateRequest(AuthorizationRequest request, NancyContext context);
        Tuple<string, object> GetAuthorizationView(AuthorizationRequest request, NancyContext context);
    }
}
