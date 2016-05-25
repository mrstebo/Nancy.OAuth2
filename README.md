# Nancy.OAuth2
A Nancy module for adding OAuth2 support.

Based on work by [thecodejunkie](https://github.com/thecodejunkie/Nancy.OAuth2)

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

Override the `TokenEndpointService`

```csharp
public class TokenEndpointService : ITokenEndpointService
{
    public OAuthValidationResult ValidateRequest(TokenRequest request, NancyContext context)
    {
        // Only allow certain grant types
        switch (request.GrantType)
        {
            case GrantTypes.Password:
                // Check to see if the client credentials are valid (usually stored in Authorization header)
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


## Todo

- Implement the Authorization module.


_Any contributions will be greatly appreciated!_
