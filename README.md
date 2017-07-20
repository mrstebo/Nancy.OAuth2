[![Build status](https://ci.appveyor.com/api/projects/status/850jf5qhe5gxnepo/branch/master?svg=true)](https://ci.appveyor.com/project/mrstebo/nancy-oauth2/branch/master) [![MyGet Prerelease](https://img.shields.io/myget/mrstebo/v/Nancy.OAuth2.svg?label=MyGet_Prerelease)](https://www.myget.org/feed/mrstebo/package/nuget/Nancy.OAuth2) [![NuGet Version](https://img.shields.io/nuget/v/Nancy.OAuth2.svg)](https://www.nuget.org/packages/Nancy.OAuth2/)

# Nancy.OAuth2
A Nancy module for adding OAuth2 support.

Based on work by [thecodejunkie](https://github.com/thecodejunkie/Nancy.OAuth2)

Nancy.OAuth2 is available via [NuGet](https://www.nuget.org/packages/Nancy.OAuth2/):
```PowerShell
Install-Package Nancy.OAuth2
```

## Getting Started

*Note: In the following examples, `IOAuthService` and `IUserService` would be your own implementations*

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

        // If you are using the authorization_code grant, then register your implementation
        container.Register<IAuthorizationEndpointService, AuthorizationEndpointService>();

        // Register your implementation of the ITokenEndpointService
        container.Register<ITokenEndpointService, TokenEndpointService>();

        base.RequestStartup(container, pipelines, context);
    }
}
```

Create a class that implements the `ITokenEndpointService` interface.

```csharp
public class TokenEndpointService : ITokenEndpointService
{
    private readonly IOAuthService _oauthService;
    private readonly IUserService _userService;

    public TokenEndpointService(
        IOAuthService oauthService,
        IUserService userService)
    {
        _oauthService = oauthService;
        _userService = userService;
    }

    public OAuthValidationResult ValidateRequest(TokenRequest request, NancyContext context)
    {
        if (!IsValidClient(request.ClientId, request.ClientSecret))
            return ErrorType.InvalidClient;

        // Only allow certain grant types
        switch (request.GrantType)
        {
            case GrantTypes.Password:
                return ValidatePasswordGrant(request);

            case GrantTypes.Authorization:
                return ValidateAuthorizationCodeGrant(request);

            default:
                return ErrorType.InvalidGrant;
        }
    }

    public TokenResponse CreateTokenResponse(TokenRequest request, NancyContext context)
    {
        // Build a token and store it somewhere so you can validate it later
        return BuildTokenResponse(request, context);
    }

    private bool IsValidClient(string clientId, string clientSecret)
    {
        var client = _oauthService.FindClientById(request.ClientId);

        return client != null && client.ClientSecret == clientSecret;
    }

    private OAuthValidationResult ValidatePasswordGrant(TokenRequest request)
    {
        var user = _userService.FindUserByUsername(request.Username);

        return user == null || user.HasValidPassword(request.Password)
            ? ErrorType.InvalidGrant
            : ErrorType.None;
    }

    private OAuthValidationResult ValidateAuthorizationCodeGrant(TokenRequest request)
    {
        var authCode = _oauthService.FindAuthorizationCode(request.Code);

        return authCode == null
            ? ErrorType.InvalidGrant
            : ErrorType.None;
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
