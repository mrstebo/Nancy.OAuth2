using Nancy.OAuth2.Models;

namespace Nancy.OAuth2.Services
{
    public class NullTokenEndpointService : ITokenEndpointService
    {
        public OAuthValidationResult ValidateRequest(TokenRequest request, NancyContext context)
        {
            throw new System.NotImplementedException();
        }

        public TokenResponse CreateTokenResponse(TokenRequest request, NancyContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
