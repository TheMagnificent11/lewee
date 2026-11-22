# Lewee.Auth.Api

FastEndpoints integration and HTTP contracts for Lewee authentication operations.

## Features

- Exposes user provisioning through a reusable FastEndpoints endpoint
- Maps HTTP requests to `Lewee.Auth.Application` commands
- Uses the standard Lewee command-result HTTP response mapping
- Supports first-login provisioning before an application session is established

## Dependencies

- [Lewee.Auth.Application](../Lewee.Auth.Application/README.md) for user provisioning
- [Lewee.Infrastructure.FastEndpoints](../Lewee.Infrastructure.FastEndpoints/README.md) for endpoint base classes

## Configuration

Register the auth application layer and include this assembly when configuring FastEndpoints:

```cs
using FastEndpoints;
using Lewee.Auth.Api;
using Lewee.Auth.Application;

services
    .AddLeweeAuthApplication()
    .AddFastEndpoints(options =>
    {
        options.Assemblies = [typeof(CreateUserRequest).Assembly];
    });
```

Configure the normal Lewee MediatR, persistence, correlation, and FastEndpoints middleware in the hosting API.

## Endpoints

| Method | Route | Authentication | Request |
|--------|-------|----------------|---------|
| `POST` | `/users` | Anonymous | `CreateUserRequest` |

```cs
var request = new CreateUserRequest
{
    ExternalUserId = externalUserId,
};
```

The endpoint dispatches `CreateUserCommand`. Provisioning is idempotent and creates users without tenant
memberships.

## Main Types

| Type | Purpose |
|------|---------|
| `CreateUserRequest` | HTTP request containing the external user identifier |
| `CreateUserEndpoint` | Handles anonymous first-login user provisioning |
