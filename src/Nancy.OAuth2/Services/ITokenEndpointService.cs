using Nancy.OAuth2.Models;

namespace Nancy.OAuth2.Services
{
    public interface ITokenEndpointService
    {
        OAuthValidationResult ValidateRequest(TokenRequest request, NancyContext context);
        TokenResponse CreateTokenResponse(TokenRequest request, NancyContext context);
    }
}
