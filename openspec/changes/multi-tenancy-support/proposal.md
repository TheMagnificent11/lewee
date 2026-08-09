## Why

The sample application currently treats every Keycloak-authenticated principal as a standalone `User` stored in the `sto` (Pizzeria Store) schema, with no concept of a tenant/organisation that owns that user or the data they can see. Keycloak already issues identities, but the plan (per issue #541) is to keep authorization data out of Keycloak and instead model it in the application database, behind a new `Tenant` aggregate root that owns one or more `User`s. Today `User`, `UserByExternalIdSpecification`, and `UserCreatedEvent` live in `Pizzeria.Store.Domain`/`Pizzeria.Store.Data`, coupling an authentication/authorization concern to the pizza-ordering bounded context and making it impossible to reuse across other Lewee-based applications. Extracting these into new `Lewee.Auth.*` framework packages, introducing `Tenant` as the aggregate root, and moving the corresponding table(s) into a new `auth` schema establishes the foundation multi-tenancy needs before further tenant-scoped behaviour (e.g. `ITenantRequest`/`TenantLoggingBehavior`, already present in `Lewee.Application`) can be wired end-to-end.

## What Changes

- Add four new framework packages: `Lewee.Auth.Domain`, `Lewee.Auth.Infrastructure.Data`, `Lewee.Auth.Application`, `Lewee.Auth.Api`.
- **BREAKING**: Move `User`, `UserByExternalIdSpecification`, and `UserCreatedEvent` out of `Pizzeria.Store.Domain` into `Lewee.Auth.Domain`, renaming `UserCreatedEvent` consistently with its new home.
- Add a new `Tenant : AggregateRoot` to `Lewee.Auth.Domain`; `User` becomes a child entity of `Tenant` (no longer an aggregate root in its own right) and gains a required `TenantId` foreign key.
- Add a new `AuthDbContext` in `Lewee.Auth.Infrastructure.Data` (schema `auth`), with a `Tenants` `DbSet` (replacing the `Users` `DbSet` currently on `StoreDbContext`) and an EF Core `UserConfiguration`/`TenantConfiguration` reflecting the new aggregate boundary; add the EF migration(s) required to create the `auth` schema tables and migrate/move existing `sto.Users` data into `auth.Users`/`auth.Tenants`.
- **BREAKING**: Remove the `Users` `DbSet` from `Pizzeria.Store.Data.StoreDbContext`.
- Move `CreateCustomerCommand` (and its validator/handler) from `Pizzeria.Store.Application.Customers` to `Lewee.Auth.Application`, renaming it to `CreateUserCommand`.
- Move `CreateCustomerEndpoint` from `Pizzeria.Store.Api.Customers` to `Lewee.Auth.Api`, renaming it to `CreateUserEndpoint`; add a project reference from `Pizzeria.Store.Api` to `Lewee.Auth.Api` so the endpoint is still exposed by the sample API.
- Update `Pizzeria.Store.Web`'s `OnTokenValidated` handler (via `AddKeycloakAuthenticationForWebApp`) to call the new create-user endpoint instead of the old create-customer endpoint.
- Evolve the database migration/seeding story so a Lewee-level configuration/seeding component can migrate `AuthDbContext` in addition to application-specific `DbContext`s, including seeding an initial administrative tenant/user and resolving/assigning its Keycloak user ID as the `User.ExternalId` on first run.

## Capabilities

### New Capabilities
- `auth/tenant-management`: Defines the `Tenant` aggregate root, its relationship to `User` child entities, and the invariants around tenant-scoped user membership.
- `auth/user-provisioning`: Defines how a `User` is created/provisioned for a `Tenant` from an external (Keycloak) identity, replacing the sample's `CreateCustomerCommand`/`CreateCustomerEndpoint` flow with `CreateUserCommand`/`CreateUserEndpoint` in the new `Lewee.Auth.Application`/`Lewee.Auth.Api` packages.
- `auth/data-persistence`: Defines the `auth` schema, `AuthDbContext`, and the migration path for moving `User` data out of `sto.Users` into `auth.Users`/`auth.Tenants`.

### Modified Capabilities
- None (no existing `openspec/specs/` capabilities are defined yet for this repository).

## Impact

- **Affected code**:
  - New projects: `src/Lewee.Auth.Domain`, `src/Lewee.Auth.Infrastructure.Data`, `src/Lewee.Auth.Application`, `src/Lewee.Auth.Api` (and corresponding `tests/Lewee.Auth.*.Tests.Unit` projects), registered in `lewee.slnx`.
  - `sample/Pizzeria.Store.Domain/User.cs`, `UserByExternalIdSpecification.cs`, `UserCreatedEvent.cs` (removed, replaced by `Lewee.Auth.Domain` equivalents).
  - `sample/Pizzeria.Store.Data/StoreDbContext.cs` (remove `Users` `DbSet`), `sample/Pizzeria.Store.Data/Configuration/UserConfiguration.cs` (removed).
  - `sample/Pizzeria.Store.Application/Customers/**` and `sample/Pizzeria.Store.Api/Customers/**` (removed, replaced by `Lewee.Auth.Application`/`Lewee.Auth.Api` equivalents), `sample/Pizzeria.Store.Contracts/Users/**` (updated/renamed DTOs as needed).
  - `sample/Pizzeria.Store.Web/TokenValidatedContextExtensions.cs` and `Program.cs` (call the new create-user endpoint).
  - `sample/Pizzeria.Configuration/**` (extended, or superseded by a new `Lewee.Configuration`, to migrate/seed `AuthDbContext` alongside `StoreDbContext`).
  - EF Core migrations: a new initial migration for `AuthDbContext` (`auth` schema) and a `Pizzeria.Store.Data` migration removing the `Users` table from the `sto` schema.
- **Breaking change**: Consumers of `Pizzeria.Store.Domain.User`/`UserCreatedEvent`/`UserByExternalIdSpecification`, and of `StoreDbContext.Users`, must move to the new `Lewee.Auth.Domain`/`Lewee.Auth.Infrastructure.Data` types. `CreateCustomerCommand`/`CreateCustomerEndpoint`/`CreateCustomerRequest` are renamed/relocated. This is a framework and sample breaking change; acceptable pre-1.0 per `decision-making.instructions.md`.
- **Dependencies**: No new third-party NuGet packages are anticipated; the new packages reuse existing dependencies (`Lewee.Domain`, `Lewee.Infrastructure.Data`, `Lewee.Application`, `Lewee.Infrastructure.FastEndpoints`, EF Core, MediatR, FluentValidation).

## Note

This change captures the **implementation plan only** (proposal, design, specs, tasks) as requested by issue #542. No production code is modified as part of this change; `tasks.md` tracks the follow-up implementation work.
