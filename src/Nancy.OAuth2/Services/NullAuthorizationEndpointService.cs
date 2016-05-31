using System;
using Nancy.OAuth2.Models;

namespace Nancy.OAuth2.Services
{
    public class NullAuthorizationEndpointService : IAuthorizationEndpointService
    {
        public string GenerateAuthorizationToken(AuthorizationRequest request, NancyContext context)
        {
            throw new NotImplementedException();
        }

        public OAuthValidationResult ValidateRequest(AuthorizationRequest request, NancyContext context)
        {
            throw new NotImplementedException();
        }

        public Tuple<string, object> GetAuthorizationView(AuthorizationRequest request, NancyContext context)
        {
            throw new NotImplementedException();
        }
    }
}
