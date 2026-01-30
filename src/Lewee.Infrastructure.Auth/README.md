# Lewee.Infrastructure.Auth

The intention of this package is help setup authentication and authorization.

At the moment, it's only purpose is to setup the [AuthenticatedUserService](./AuthenticatedUserService.cs).

## Dependencies

- ASP.Net Framework (for `IHttpContextAccessor`)
- [Lewee.Common](../Lewee.Common/README.md)

## Configuration

### Authenticated User Configuration

```cs
services.ConfigureAuthenticatedUserService();
```

## Usage

### Authenticated User

Inject `IAuthenticatedUserService` into an services that need to obtain the `UserId` of a user (this is the value stored as the [name identifier claim](http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier) in a JWT).

[Lewee.Infrastructure.Data](../Lewee.Infrastructure.Data/README.md) uses this service to populate the created/modified by user ID on entity table records.

