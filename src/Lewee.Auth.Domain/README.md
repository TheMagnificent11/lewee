# Lewee.Auth.Domain

Domain models for application-owned authentication and multi-tenant authorization data.

## Features

- Independent `User` and `Tenant` aggregate roots
- Globally unique external user identities
- Zero-or-more tenant memberships per user
- Idempotent tenant assignment and removal
- A direct `IsSiteAdministrator` flag on `User`, independent of tenant or role
- A globally-unique `Role` catalog, defined by a site administrator and assignable to any tenant's memberships
- Idempotent role assignment/removal on `TenantMembership`, supporting multiple roles per membership
- Domain events for user, tenant, membership, and role lifecycle changes

## Dependencies

- [Lewee.Domain](../Lewee.Domain/README.md) for aggregate roots, entities, domain events, and specifications

## Usage

Create users independently of tenants, then manage memberships separately:

```cs
var tenant = Tenant.Create("STORE1", "Store One", correlationId);
var user = User.Create(externalUserId, correlationId);

user.AssignToTenant(tenant.Id, correlationId);
user.RemoveFromTenant(tenant.Id, correlationId);
```

Tenant codes are unique secondary identifiers with a maximum length of 10 characters. Tenant names have a maximum
length of 200 characters, and external user IDs have a maximum length of 100 characters.

A site administrator is identified directly by `User.IsSiteAdministrator`, independent of any tenant or role. There
is no domain method to set this flag - given how rarely it changes, it is expected to be set directly against the
database (e.g. a SQL `UPDATE`).

Roles are defined once, globally, by a site administrator, and can be assigned to any tenant's memberships:

```cs
var role = Role.Create("MANAGER", "Manager", correlationId);

user.AssignRole(tenant.Id, role.Id, correlationId);
user.RemoveRole(tenant.Id, role.Id, correlationId);
```

A single `TenantMembership` can hold zero, one, or many roles at once. Role codes are globally unique (not scoped to
a tenant), with a maximum length matching `Role.FieldLengths.Code`; role names have a maximum length matching
`Role.FieldLengths.Name` and are not required to be unique.

## Main Types

| Type | Purpose |
| ------ | --------- |
| `Tenant` | Identifies an organization by ID, unique code, and name |
| `User` | Represents an external identity independently of tenant membership; exposes `IsSiteAdministrator` |
| `TenantMembership` | Associates a user with a tenant, and zero or more roles |
| `Role` | A globally-unique, site-administrator-defined role that any tenant may assign to its members |
| `UserByExternalIdSpecification` | Finds a user by globally unique external identity |
| `TenantCreatedEvent` | Records tenant creation, including its code and name |
| `TenantMembershipCreatedEvent` | Records assignment of a user to a tenant |
| `TenantMembershipRemovedEvent` | Records removal of a user from a tenant |
| `RoleDefinedEvent` | Records definition of a new role |
| `TenantMembershipRoleAssignedEvent` | Records assignment of a role to a tenant membership |
| `TenantMembershipRoleRemovedEvent` | Records removal of a role from a tenant membership |

Persistence and application orchestration are provided by
[Lewee.Auth.Infrastructure.Data](../Lewee.Auth.Infrastructure.Data/README.md) and
[Lewee.Auth.Application](../Lewee.Auth.Application/README.md).
