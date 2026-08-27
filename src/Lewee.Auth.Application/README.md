# Lewee.Auth.Application

Application-layer services for provisioning users from external identity providers.

## Features

- Validates external user identifiers
- Creates users without requiring tenant membership
- Treats repeated provisioning requests as idempotent
- Publishes and handles user domain events through MediatR
- Provides dependency-injection registration for the auth application layer

## Dependencies

- [Lewee.Application](../Lewee.Application/README.md) for CQRS, validation, and pipeline behaviors
- [Lewee.Auth.Domain](../Lewee.Auth.Domain/README.md) for user aggregates and specifications

## Configuration

Register the command handlers, validators, and domain event handlers:

```cs
using Lewee.Auth.Application;

services.AddLeweeAuthApplication();
```

The host must also register an `IRepository<User>` implementation. The
[Lewee.Auth.Infrastructure.Data](../Lewee.Auth.Infrastructure.Data/README.md) package supplies the corresponding
`AuthDbContext`.

## Usage

Dispatch `CreateUserCommand` with the stable subject identifier supplied by the external identity provider:

```cs
var result = await mediator.Send(
    new CreateUserCommand(externalUserId),
    cancellationToken);
```

The command validates the identifier against `User.FieldLengths.ExternalId`, returns success when the user already
exists, and creates a new user with no tenant memberships otherwise. Tenant assignment remains a separate domain
operation.

## Main Types

| Type | Purpose |
|------|---------|
| `AuthApplicationConfiguration` | Registers auth application services |
| `CreateUserCommand` | Idempotently provisions a user from an external identity |
| `UserCreatedEventHandler` | Handles user-created domain notifications |
| `UserDto` | Represents user data returned by application operations |
