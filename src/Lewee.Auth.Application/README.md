# Lewee.Auth.Application

Application-layer services for provisioning users from external identity providers and authorizing
`MediatR` commands/queries against site-administrator and tenant-role checks.

## Features

- Validates external user identifiers
- Creates users without requiring tenant membership
- Treats repeated provisioning requests as idempotent
- Publishes and handles user domain events through MediatR
- Maintains a per-tenant role lookup read model, kept in sync via domain event handlers
- Restricts commands/queries to a site administrator via `IAdministratorRequest`
- Restricts commands/queries to callers holding a tenant role via `ITenantRoleRequest`
- Provides dependency-injection registration for the auth application layer, and separately, for authorization

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

Authorization is opt-in and registered separately, since not every consumer requires it:

```cs
services.AddLeweeApplicationAuth();
```

This registers `TenantLoggingBehavior`, `AdministratorAuthorizationBehavior`, and `TenantRoleAuthorizationBehavior`
as `MediatR` pipeline behaviors, in addition to the baseline behaviors registered by
`Lewee.Application.ApplicationConfiguration.AddPipelineBehaviors`.

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

### Authorization

Restrict a command or query to a site administrator by implementing `IAdministratorRequest`:

```cs
public sealed record DefineRoleCommand(string Code, string Name) : ICommand, IAdministratorRequest;
```

`AdministratorAuthorizationBehavior` resolves the caller via `IAuthenticatedUserService`, short-circuits with
`ResultStatus.Unauthenticated` when no caller is resolved, and short-circuits with `ResultStatus.Unauthorized`
unless the caller's `User.IsSiteAdministrator` is `true`.

Restrict a command or query to callers holding one of a set of tenant roles by implementing `ITenantRoleRequest`:

```cs
public sealed record AssignRoleCommand(Guid TenantId, Guid RoleId) : ICommand, ITenantRoleRequest
{
    public IReadOnlyCollection<string> SatisfyingRoles => ["MANAGER"];
}
```

`TenantRoleAuthorizationBehavior` resolves the caller, short-circuits with `ResultStatus.Unauthenticated` when no
caller is resolved, and short-circuits with `ResultStatus.Unauthorized` unless the caller is a member of
`request.TenantId` and holds at least one of `SatisfyingRoles` for that tenant. The membership/role check is
served by `TenantMembershipRolesQueryProjection`, a read model kept in sync by
`TenantAuthorizationQueryProjectionHandler` reacting to membership/role domain events, avoiding a per-request join.

Requests that do not implement `IAdministratorRequest`/`ITenantRoleRequest` are unaffected by these behaviors.

## Main Types

| Type | Purpose |
| ------ | --------- |
| `AuthApplicationConfiguration` | Registers auth application services (handlers, validators, event handlers) |
| `ApplicationAuthConfiguration` | Registers the opt-in authorization pipeline behaviors |
| `CreateUserCommand` | Idempotently provisions a user from an external identity |
| `UserCreatedEventHandler` | Handles user-created domain notifications |
| `UserDto` | Represents user data returned by application operations |
| `IAdministratorRequest` | Marker interface restricting a command/query to a site administrator |
| `ITenantRoleRequest` | Marker interface restricting a command/query to callers holding a tenant role |
| `AdministratorAuthorizationBehavior` | Pipeline behavior authorizing `IAdministratorRequest`s |
| `TenantRoleAuthorizationBehavior` | Pipeline behavior authorizing `ITenantRoleRequest`s |
| `TenantLoggingBehavior` | Pipeline behavior adding the tenant ID to the logging scope of `ITenantRequest`s |
| `TenantMembershipRolesQueryProjection` | Read model of a user's tenant membership/roles |
| `TenantAuthorizationQueryProjectionHandler` | Keeps the projection in sync with membership/role domain events |
