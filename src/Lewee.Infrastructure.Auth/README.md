# Lewee.Infrastructure.Auth

Authentication and authorization infrastructure components for domain-driven design applications. This package provides infrastructure layer services for retrieving authenticated user context and propagating authentication tokens to external HTTP requests.

## Dependencies

- Microsoft.AspNetCore.App (framework reference for `IHttpContextAccessor` and authentication infrastructure)
- [Lewee.Common](../Lewee.Common/README.md) - Shared utilities including `IAuthenticatedUserService` interface

## Features

This infrastructure package provides:

- **Authenticated User Service**: Retrieves the current user's ID from the authentication context
- **Authentication Token Handler**: HTTP message handler that automatically adds Bearer tokens to outgoing HTTP requests

## Configuration

### Authenticated User Service

Register the authenticated user service to access the current user's ID:

```cs
using Lewee.Infrastructure.Auth;

// Server-side applications must register IHttpContextAccessor first
services.AddHttpContextAccessor();

// Then register the authenticated user service
services.AddAuthenticatedUserService();
```

**Important**: Server-side Blazor and ASP.NET Core applications must call `AddHttpContextAccessor()` before registering the authenticated user service. This is required because `IHttpContextAccessor` is a server-side concept and cannot be registered by a library that supports WebAssembly.

This registers:
- `IAuthenticatedUserService` - Service for retrieving the authenticated user's ID from the current HTTP context

### Authentication Token Delegating Handler

The `AuthTokenDelegatingHandler` is typically configured automatically by other infrastructure packages (such as [Lewee.Infrastructure.Refit](../Lewee.Infrastructure.Refit/README.md)) but can be registered manually if needed:

```cs
services.AddTransient<AuthTokenDelegatingHandler>();
```

## Usage

### Authenticated User Service

Inject `IAuthenticatedUserService` into services that need to obtain the `UserId` of the authenticated user. The user ID is retrieved from the [NameIdentifier claim](http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier) in the authentication context (typically from a JWT token).

```cs
using Lewee.Common;

public class MyService
{
    private readonly IAuthenticatedUserService authenticatedUserService;

    public MyService(IAuthenticatedUserService authenticatedUserService)
    {
        this.authenticatedUserService = authenticatedUserService;
    }

    public void DoSomething()
    {
        var userId = this.authenticatedUserService.UserId;
        if (userId is not null)
        {
            // Use the authenticated user's ID
        }
    }
}
```

**Note**: [Lewee.Infrastructure.Data](../Lewee.Infrastructure.Data/README.md) uses this service to automatically populate the created/modified by user ID on entity table records.

### Authentication Token Handler

The `AuthTokenDelegatingHandler` is used to automatically add Bearer authentication tokens to outgoing HTTP requests. It:

1. Retrieves the `access_token` from the current authentication context (`HttpContext`)
2. Adds it as a `Bearer` token in the `Authorization` header
3. Logs warnings when tokens are unavailable

This handler is automatically configured by infrastructure packages like [Lewee.Infrastructure.Refit](../Lewee.Infrastructure.Refit/README.md).

## Infrastructure Components

| Component | Description |
|-----------|-------------|
| **[AuthenticatedUserServiceConfiguration.cs](./AuthenticatedUserServiceConfiguration.cs)** | Extension method for registering authenticated user service |
| **[AuthenticatedUserService.cs](./AuthenticatedUserService.cs)** | Implementation of `IAuthenticatedUserService` that retrieves user ID from HTTP context |
| **[AuthTokenDelegatingHandler.cs](./AuthTokenDelegatingHandler.cs)** | HTTP message handler for automatic authentication token propagation |

## Integration with Lewee DDD Layers

This infrastructure package integrates with other Lewee packages across the DDD layers:

| Package | Layer | Integration |
|---------|-------|-------------|
| **[Lewee.Common](../Lewee.Common/README.md)** | Cross-cutting | Defines `IAuthenticatedUserService` interface |
| **[Lewee.Infrastructure.Data](../Lewee.Infrastructure.Data/README.md)** | Infrastructure | Uses `IAuthenticatedUserService` for audit tracking |
| **[Lewee.Infrastructure.Refit](../Lewee.Infrastructure.Refit/README.md)** | Infrastructure | Uses `AuthTokenDelegatingHandler` for API client authentication |

## Best Practices

1. **Register Early**: Call `AddAuthenticatedUserService()` during application startup before services that depend on `IAuthenticatedUserService`
2. **Check for Null**: Always check if `UserId` is null before using it, as unauthenticated requests will return null
3. **Authentication Required**: Ensure users are properly authenticated before services attempt to access `IAuthenticatedUserService.UserId`
4. **HTTP Context Required**: Both components require an active `HttpContext`, which is available in web applications but not in background services or console applications

