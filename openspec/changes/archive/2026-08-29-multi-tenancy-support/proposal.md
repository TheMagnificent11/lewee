## Why

The sample application currently treats every Keycloak-authenticated principal as a standalone `User` stored in the `sto` (Pizzeria Store) schema, with no concept of a tenant/organisation that the user may belong to, nor of the data they can see as a result. Keycloak already issues identities, but the plan (per issue #541) is to keep authorization data out of Keycloak and instead model it in the application database, behind a new `Tenant` aggregate root. A `User` can be a member of zero tenants (e.g. immediately after first login, before being invited/assigned anywhere) or of one or more tenants, so `User` must remain its own aggregate root, related to `Tenant` via an explicit membership rather than being owned by it. Today `User`, `UserByExternalIdSpecification`, and `UserCreatedEvent` live in `Pizzeria.Store.Domain`/`Pizzeria.Store.Data`, coupling an authentication/authorization concern to the pizza-ordering bounded context and making it impossible to reuse across other Lewee-based applications. Extracting these into new `Lewee.Auth.*` framework packages, introducing `Tenant` as a second aggregate root alongside `User`, and moving the corresponding table(s) into a new `auth` schema establishes the foundation multi-tenancy needs before further tenant-scoped behaviour (e.g. `ITenantRequest`/`TenantLoggingBehavior`, already present in `Lewee.Application`) can be wired end-to-end.

## What Changes

- Add four new framework packages: `Lewee.Auth.Domain`, `Lewee.Auth.Infrastructure.Data`, `Lewee.Auth.Application`, `Lewee.Auth.Api`.
- **BREAKING**: Move `User`, `UserByExternalIdSpecification`, and `UserCreatedEvent` out of `Pizzeria.Store.Domain` into `Lewee.Auth.Domain`. `User` remains an `AggregateRoot` (it is not owned by `Tenant`).
- Add a new `Tenant : AggregateRoot` to `Lewee.Auth.Domain`, as a second, independent aggregate root with a unique, maximum-10-character `Code` for lookup. A `User` can belong to zero, one, or many `Tenant`s; this membership is modeled as a `TenantMembership` child entity owned by `User` (not a `TenantId` foreign key on `User`, and not a `Users` collection owned by `Tenant`).
- Add a new `AuthDbContext` in `Lewee.Auth.Infrastructure.Data` (schema `auth`), with `Tenants` and `Users` `DbSet`s (replacing the `Users` `DbSet` currently on `StoreDbContext`) and EF Core configuration for `Tenant`, `User`, and the `TenantMembership` join table; add the EF migration(s) required to create the `auth` schema tables and migrate existing `sto.Users` data into `auth.Users`.
- **BREAKING**: Remove the `Users` `DbSet` from `Pizzeria.Store.Data.StoreDbContext`.
- Move `CreateCustomerCommand` (and its validator/handler) from `Pizzeria.Store.Application.Customers` to `Lewee.Auth.Application`, renaming it to `CreateUserCommand`. The command continues to accept only the external user identifier; it creates a `User` with no tenant membership. Assigning a `User` to one or more `Tenant`s is separate, later work (see Non-Goals in `design.md`).
- Move `CreateCustomerEndpoint` from `Pizzeria.Store.Api.Customers` to `Lewee.Auth.Api`, renaming it to `CreateUserEndpoint`; host it in a dedicated sample `Pizzeria.Auth.Api`.
- Add a `Pizzeria.Bff` reverse proxy that routes auth requests to `Pizzeria.Auth.Api` and store requests to `Pizzeria.Store.Api`. Update the web application's API client, SSE connection, and first-login provisioning callback to call only the BFF.
- Evolve the database migration/seeding story so a Lewee-level configuration/seeding component can migrate `AuthDbContext` in addition to application-specific `DbContext`s, including seeding an initial administrative tenant/user and resolving/assigning its Keycloak user ID as the `User.ExternalId` on first run.

## Capabilities

### New Capabilities

- `auth/tenant-management`: Defines the `Tenant` aggregate root and the invariants around a `User`'s membership (zero, one, or many `Tenant`s) via `TenantMembership`.
- `auth/user-provisioning`: Defines how a `User` is created from an external (Keycloak) identity with no tenant membership, replacing the sample's `CreateCustomerCommand`/`CreateCustomerEndpoint` flow with `CreateUserCommand`/`CreateUserEndpoint` in the new `Lewee.Auth.Application`/`Lewee.Auth.Api` packages.
- `auth/data-persistence`: Defines the `auth` schema, `AuthDbContext`, and the migration path for moving `User` data out of `sto.Users` into `auth.Users`.

### Modified Capabilities

- None (no existing `openspec/specs/` capabilities are defined yet for this repository).

## Impact

- **Affected code**:
  - New projects: `src/Lewee.Auth.Domain`, `src/Lewee.Auth.Infrastructure.Data`, `src/Lewee.Auth.Application`, `src/Lewee.Auth.Api` (and corresponding `tests/Lewee.Auth.*.Tests.Unit` projects), registered in `lewee.slnx`.
  - `sample/Pizzeria.Store.Domain/User.cs`, `UserByExternalIdSpecification.cs`, `UserCreatedEvent.cs` (removed, replaced by `Lewee.Auth.Domain` equivalents).
  - `sample/Pizzeria.Store.Data/StoreDbContext.cs` (remove `Users` `DbSet`), `sample/Pizzeria.Store.Data/Configuration/UserConfiguration.cs` (removed).
  - `sample/Pizzeria.Store.Application/Customers/**` and `sample/Pizzeria.Store.Api/Customers/**` (removed, replaced by `Lewee.Auth.Application`/`Lewee.Auth.Api` equivalents), `sample/Pizzeria.Store.Contracts/Users/**` (updated/renamed DTOs as needed).
  - New sample hosts: `sample/Pizzeria.Auth.Api` and `sample/Pizzeria.Bff`.
  - `sample/Pizzeria.Store.Web/TokenValidatedContextExtensions.cs` and `Program.cs` (call the BFF).
  - `sample/Pizzeria.Configuration/**` (extended, or superseded by a new `Lewee.Configuration`, to migrate/seed `AuthDbContext` alongside `StoreDbContext`).
  - EF Core migrations: a new initial migration for `AuthDbContext` (`auth` schema) and a `Pizzeria.Store.Data` migration removing the `Users` table from the `sto` schema.
- **Breaking change**: Consumers of `Pizzeria.Store.Domain.User`/`UserCreatedEvent`/`UserByExternalIdSpecification`, and of `StoreDbContext.Users`, must move to the new `Lewee.Auth.Domain`/`Lewee.Auth.Infrastructure.Data` types. `CreateCustomerCommand`/`CreateCustomerEndpoint`/`CreateCustomerRequest` are renamed/relocated. This is a framework and sample breaking change; acceptable pre-1.0 per `decision-making.instructions.md`.
- **Dependencies**: The framework packages reuse existing dependencies (`Lewee.Domain`, `Lewee.Infrastructure.Data`, `Lewee.Application`, `Lewee.Infrastructure.FastEndpoints`, EF Core, MediatR, FluentValidation). The sample BFF uses `Microsoft.Extensions.ServiceDiscovery.Yarp`.

## Note

This change captures the **implementation plan only** (proposal, design, specs, tasks) as requested by issue #542. No production code is modified as part of this change; `tasks.md` tracks the follow-up implementation work.
