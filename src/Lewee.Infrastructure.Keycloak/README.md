# Lewee.Infrastructure.Keycloak

This package provides authentication infrastructure for integrating Keycloak with ASP.NET Core Blazor applications.

## Features

- Simplified Keycloak OpenID Connect authentication setup
- Cookie-based authentication scheme
- Customizable authentication events
- Sign-out endpoint mapping

## Usage

### Adding Authentication Services

```csharp
services.AddKeycloakAuthentication(
    keycloakServiceName: "auth-server",
    keycloakRealmName: "my-realm",
    keycloakClientId: "my-client-id",
    events: new OpenIdConnectEvents
    {
        OnTokenValidated = async context =>
        {
            // Custom logic when token is validated
        }
    });
```

### Mapping Sign-Out Endpoint

```csharp
app.MapKeycloakSignOut("/signout");
```

## Configuration

The package automatically configures:
- Cookie authentication as the default scheme
- OpenID Connect as the challenge scheme
- Authorization code flow with PKCE
- Token saving
- Claims mapping (username and roles)
- Required scopes (openid, profile, email)

## Dependencies

- `Aspire.Keycloak.Authentication`: Keycloak integration for .NET Aspire
- `Microsoft.AspNetCore.Authentication.OpenIdConnect`: OpenID Connect authentication
