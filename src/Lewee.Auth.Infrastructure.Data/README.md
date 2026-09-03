# Lewee.Auth.Infrastructure.Data

Entity Framework Core persistence for Lewee authentication and multi-tenant authorization data.

## Features

- Dedicated `AuthDbContext` with the `auth` default schema
- Separate tables for users, tenants, and tenant memberships
- Unique external user identities
- Unique tenant codes with a maximum length of 10 characters
- Unique user/tenant membership pairs
- Lewee audit, soft-delete, domain-event outbox, and query-projection support

## Dependencies

- [Lewee.Auth.Domain](../Lewee.Auth.Domain/README.md) for auth aggregates
- [Lewee.Infrastructure.Data](../Lewee.Infrastructure.Data/README.md) for EF Core and repository infrastructure
- `Npgsql.EntityFrameworkCore.PostgreSQL` for PostgreSQL persistence

## Configuration

Register the context and Lewee database services in an ASP.NET Core host:

```cs
using Lewee.Auth.Domain;
using Lewee.Auth.Infrastructure.Data;
using Lewee.Infrastructure.Data;
using Lewee.Infrastructure.PostgreSQL;

services
    .AddLeweePostgreSQL<AuthDbContext>(
        connectionString,
        typeof(User).Assembly,
        AuthDbContext.SchemaName)
    .AddLeweeDatabaseServices<AuthDbContext>(typeof(User).Assembly);
```

Apply the included migrations before serving requests:

```cs
await serviceProvider.MigrateDatabaseAsync<AuthDbContext>(
    seedData: true,
    cancellationToken);
```

Register an `IDatabaseSeeder<AuthDbContext>` when application-specific initial users, tenants, or memberships are
required.

## Database Model

| Table | Purpose | Important constraints |
| ------- | --------- | ----------------------- |
| `auth.Tenants` | Stores tenant aggregates | Unique `Code` |
| `auth.Users` | Stores user aggregates | Unique `ExternalId` |
| `auth.UserTenantMemberships` | Stores user-to-tenant relationships | Unique `(UserId, TenantId)` |

The initial migration also preserves legacy users by copying existing `sto.Users` rows into `auth.Users` before the
sample store migration removes the source table.

## Main Types

| Type | Purpose |
| ------ | --------- |
| `AuthDbContext` | EF Core context for auth data |
| `TenantConfiguration` | Configures tenant properties and unique code |
| `UserConfiguration` | Configures users and owned tenant memberships |
