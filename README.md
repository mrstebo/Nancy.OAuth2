# Nancy.OAuth2
A Nancy module for adding OAuth2 support.

Based on work by [thecodejunkie](https://github.com/thecodejunkie/Nancy.OAuth2)

Nancy.OAuth2 is available via [NuGet](https://www.nuget.org/packages/Nancy.OAuth2/):
```PowerShell
Install-Package Nancy.OAuth2
```

## Getting Started

Install the module
```csharp
public class Bootstrapper : DefaultNancyBootstrapper
{
    protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
    {
        // Default configuration for OAuth.Enable()
        OAuth.Enable(config =>
        {
            config.Base = "/oauth";
            config.AuthorizationRequestRoute = "/authorize";
            config.AuthorizationAllowRoute = "/allow";
            config.AuthorizationDenyRoute = "/deny";
            config.TokenRoute = "/token";
        });

        base.RequestStartup(container, pipelines, context);
    }
}
```

Create a class that implements the `ITokenEndpointService` interface.

```csharp
public class TokenEndpointService : ITokenEndpointService
{
    public OAuthValidationResult ValidateRequest(TokenRequest request, NancyContext context)
    {
        // Only allow certain grant types
        switch (request.GrantType)
        {
            case GrantTypes.Password:
                // Check to see if the client credentials are valid
                // (usually stored in Authorization header)
                break;
                // OR return ErrorType.InvalidClient

            default:
                return ErrorType.InvalidGrant;
        }
        return ErrorType.None;
    }

    public TokenResponse CreateTokenResponse(TokenRequest request, NancyContext context)
    {
        return BuildTokenResponse(request, context);
    }
}
```

### Authorization

If you want to enable the /authorize endpoints then you will need to create a
class that implements the `IAuthorizationEndpointService` interface.

```csharp
public class AuthorizationEndpointService : IAuthorizationEndpointService
{
    private readonly IOAuthService _oauthService;
    private readonly IUserService _userService;

    public AuthorizationEndpointService(
        IOAuthService oauthService,
        IUserService userService)
    {
        _oauthService = oauthService;
        _userService = userService;
    }

    public string GenerateAuthorizationToken(AuthorizationRequest request, NancyContext context)
    {
        var client = _oauthService.FindClientById(request.ClientId);
        var user = _userService.FindUserByUsername(context.CurrentUser.UserName);
        var authCode = _oauthService.CreateAuthCode(client, user);

        return authCode.Token;
    }

    public OAuthValidationResult ValidateRequest(AuthorizationRequest request, NancyContext context)
    {
        var client = _oauthService.FindClientById(request.ClientId);

        if (client == null)
            return ErrorType.InvalidClient;

        // Perform validation of the request for the client e.g.
        // - Is the RedirectUri allowed?
        // - Does it support the authorization_code grant?

        return ErrorType.None;
    }

    public Tuple<string, object> GetAuthorizationView(AuthorizationRequest request, NancyContext context)
    {
        var client = _oauthService.FindClientById(request.ClientId);
        var permissions = _oauthService.GetClientPermissions(request.ClientId);

        return new Tuple<string, object>("Authorize", new AuthorizeViewModel
        {
            Name = client.Name,
            Description = client.Description,
            Permissions = permissions
        });
    }
}
```

_Any contributions will be greatly appreciated!_
