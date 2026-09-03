# Lewee.Auth.Domain

Domain models for application-owned authentication and multi-tenant authorization data.

## Features

- Independent `User` and `Tenant` aggregate roots
- Globally unique external user identities
- Zero-or-more tenant memberships per user
- Idempotent tenant assignment and removal
- Domain events for user, tenant, and membership lifecycle changes

## Dependencies

- [Lewee.Domain](../Lewee.Domain/README.md) for aggregate roots, entities, domain events, and specifications

## Usage

Create users independently of tenants, then manage memberships separately:

```cs
var tenant = Tenant.Create("ADMIN", "Administration", correlationId);
var user = User.Create(externalUserId, correlationId);

user.AssignToTenant(tenant.Id, correlationId);
user.RemoveFromTenant(tenant.Id, correlationId);
```

Tenant codes are unique secondary identifiers with a maximum length of 10 characters. Tenant names have a maximum
length of 200 characters, and external user IDs have a maximum length of 100 characters.

## Main Types

| Type | Purpose |
| ------ | --------- |
| `Tenant` | Identifies an organization by ID, unique code, and name |
| `User` | Represents an external identity independently of tenant membership |
| `TenantMembership` | Associates a user with a tenant |
| `UserByExternalIdSpecification` | Finds a user by globally unique external identity |
| `TenantCreatedEvent` | Records tenant creation, including its code and name |
| `TenantMembershipCreatedEvent` | Records assignment of a user to a tenant |
| `TenantMembershipRemovedEvent` | Records removal of a user from a tenant |

Persistence and application orchestration are provided by
[Lewee.Auth.Infrastructure.Data](../Lewee.Auth.Infrastructure.Data/README.md) and
[Lewee.Auth.Application](../Lewee.Auth.Application/README.md).
