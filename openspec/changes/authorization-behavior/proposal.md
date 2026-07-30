## Why

Lewee currently has no way to authorize a MediatR command/query beyond authentication: any authenticated user can invoke any command or query, and there is no framework-level concept of "this request requires site-administrator access" or "this request requires the caller to hold a specific tenant role". Issue #87 calls for two authorization behaviors — administrator-only authorization and tenant-scoped claims/role authorization — implemented as MediatR pipeline behaviors, plus the underlying `Lewee.Auth` package and database schema needed to store users and their tenant roles independently of Keycloak (which is not used to carry authorization claims). This change produces the OpenSpec implementation plan for that work so it can be built incrementally and reviewed against the framework's clean-architecture constraints before implementation begins.

## What Changes

- Introduce a new `Lewee.Auth` package family (`Lewee.Auth.Domain`, `Lewee.Auth.Infrastructure.Data`, `Lewee.Auth.Application`, `Lewee.Auth.Api`) that owns the `User` aggregate, `UserCreatedEvent`, and a new `UserRole` concept, replacing the sample-app-only `Pizzeria.Store.Domain.User`.
- **BREAKING**: Move `User`, `UserByExternalIdSpecification`, and `UserCreatedEvent` out of `Pizzeria.Store.Domain` into `Lewee.Auth.Domain`; move `UserConfiguration` and the `Users` DbSet out of `Pizzeria.Store.Data.StoreDbContext` into a new `AuthDbContext` in `Lewee.Auth.Infrastructure.Data` (new `auth` schema), requiring an EF Core migration to relocate existing `sto.Users` data to `auth.Users`.
- Migrate `Pizzeria.Store.Application.Customers.CreateCustomerCommand` / `Pizzeria.Store.Api.Customers.CreateCustomerEndpoint` to `Lewee.Auth.Application.CreateUserCommand` / `Lewee.Auth.Api.CreateUserEndpoint`, and update `Pizzeria.Store.Web`'s Keycloak `OnTokenValidated` handler to call the new endpoint.
- Add `IAdministratorRequest` marker interface (in `Lewee.Application`) and an `AdministratorAuthorizationBehavior` MediatR pipeline behavior that short-circuits with a 403-equivalent `CommandResult`/`QueryResult` failure when the current authenticated user is not a site administrator.
- Add an `ITenantRoleRequest` (or similarly named) interface that declares the tenant role(s) required to authorize a command/query, backed by a `TenantRoleAuthorizationBehavior` that looks up the caller's roles for the current tenant from a fast, denormalized store (e.g., a stored/read-side query keyed by tenant ID + user ID) maintained by domain event handlers whenever user-role assignments change.
- Add API endpoints (in `Lewee.Auth.Api`) for tenant administrators to define roles and assign/revoke them for users within a tenant.

## Capabilities

### New Capabilities
- `administrator-authorization`: Defines how commands/queries opt into site-administrator-only access via `IAdministratorRequest` and how the MediatR pipeline enforces it.
- `tenant-role-authorization`: Defines how commands/queries declare required tenant role(s) via a request interface, how the pipeline behavior resolves the caller's roles for the current tenant from a fast lookup store, and how that store is kept in sync via domain events when role assignments change.
- `user-management`: Defines the relocation of the `User` aggregate and user creation flow into the new `Lewee.Auth` packages, including the `auth` schema and the Keycloak-to-Lewee-user provisioning flow.

### Modified Capabilities
- None (no existing `openspec/specs/` capabilities are defined yet for authorization or user management).

## Impact

- **Affected code**:
  - New projects: `src/Lewee.Auth.Domain`, `src/Lewee.Auth.Infrastructure.Data`, `src/Lewee.Auth.Application`, `src/Lewee.Auth.Api` (and corresponding `tests/*.Tests.Unit` projects), added to `lewee.slnx` and `Directory.Packages.props` as needed.
  - `src/Lewee.Application/Mediation/Requests/` — new `IAdministratorRequest` and tenant-role request interfaces.
  - `src/Lewee.Application/Mediation/Behaviors/` — new `AdministratorAuthorizationBehavior`, new `TenantRoleAuthorizationBehavior`, and DI registration in `ApplicationConfiguration.AddPipelineBehaviors`.
  - `sample/Pizzeria.Store.Domain`, `sample/Pizzeria.Store.Data`, `sample/Pizzeria.Store.Application/Customers`, `sample/Pizzeria.Store.Api/Customers`, `sample/Pizzeria.Store.Contracts/Users` — classes moved/removed in favor of `Lewee.Auth.*` equivalents.
  - `sample/Pizzeria.Store.Web` — Keycloak `OnTokenValidated` wiring updated to call the new `CreateUserEndpoint`.
  - `sample/Pizzeria.Configuration` (or a future `Lewee.Configuration`) — migration/seeding logic extended to migrate `AuthDbContext` and seed the initial administrator user, resolving its Keycloak external ID.
  - A new EF Core migration moving `sto.Users` data into `auth.Users`.
- **Breaking change**: Framework consumers referencing `Pizzeria.Store.Domain.User` (sample-only, not currently a published framework package) will need to reference `Lewee.Auth.Domain.User` instead; any database with existing `sto.Users` data requires the relocation migration.
- **Dependencies**: No new third-party NuGet packages anticipated; reuses existing `Lewee.Domain`, `Lewee.Application`, `Lewee.Infrastructure.Data`, `Lewee.Infrastructure.Auth`, and `Lewee.Infrastructure.FastEndpoints` building blocks.
