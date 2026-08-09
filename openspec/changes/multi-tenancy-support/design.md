## Context

See `proposal.md` - Why/What Changes for motivation and scope. Today `Pizzeria.Store.Domain` owns `User` (an `AggregateRoot`), `UserByExternalIdSpecification`, and `UserCreatedEvent`; `Pizzeria.Store.Data.StoreDbContext` (schema `sto`) exposes a `Users` `DbSet` configured by `UserConfiguration : AggregateRootConfiguration<User>`; `Pizzeria.Store.Application.Customers.CreateCustomerCommand` creates a `User` via `User.Create(externalId, correlationId)` after checking `UserByExternalIdSpecification` for an existing record; and `Pizzeria.Store.Api.Customers.CreateCustomerEndpoint` (a `CommandEndpoint<CreateCustomerRequest>`, anonymous access allowed) is invoked from `Pizzeria.Store.Web`'s `OnTokenValidated` handler on first login. `Pizzeria.Configuration` is a console app that calls `IServiceProvider.MigrateDatabaseAsync<StoreDbContext>(seedData: true, ...)` on startup (via `DatabaseConfiguration` in `Lewee.Infrastructure.Data`), which connects with retry/back-off, runs `Database.MigrateAsync()`, then invokes an `IDatabaseSeeder<T>` if registered.

`Lewee.Application` already has `ITenantRequest` (a `TenantId` marker interface) and `TenantLoggingBehavior<TRequest, TResponse>`, so tenant-aware logging exists but nothing yet resolves *which* tenant a request belongs to from an authenticated principal, and no `Tenant` concept exists in the domain layer.

Constraints:
- `Lewee.Domain` has no dependencies on other layers; `Tenant`/`User` domain classes must not reference EF Core, Correlate, or ASP.NET Core.
- `Lewee.Application` depends only on `Lewee.Domain` (+ MediatR/FluentValidation/Correlate abstractions, per existing `Lewee.Application` dependencies).
- `Lewee.Infrastructure.Data` classes (`ApplicationDbContext<TContext>`, `AggregateRootConfiguration<T>`, `DatabaseConfiguration.MigrateDatabaseAsync<T>`) are reused as-is; a new `AuthDbContext : ApplicationDbContext<AuthDbContext>` follows the same pattern as `StoreDbContext`.
- This is a framework (pre-1.0) breaking change; per `decision-making.instructions.md`, backward compatibility is a lower priority than establishing the correct long-term shape.

## Goals / Non-Goals

**Goals:**
- Land `Tenant` as the aggregate root owning `User` child entities in a new `Lewee.Auth.Domain` package, reusable by any Lewee-based application (not just Pizzeria).
- Give `Tenant`/`User` their own database context (`AuthDbContext`, schema `auth`) and migration(s), separate from `StoreDbContext` (schema `sto`).
- Preserve the existing "create-on-first-login, idempotent by external ID" behavior, but scope uniqueness of the external ID to a `Tenant` instead of globally.
- Keep the sample application's outward behavior (anonymous create-user endpoint invoked from `OnTokenValidated`) working end-to-end against a single default/seeded tenant, without requiring a tenant-selection UI in this iteration.
- Provide a clear, minimally-invasive path for migrating pre-existing `sto.Users` rows into `auth.Users`/`auth.Tenants`.

**Non-Goals:**
- Building tenant management UI/UX (tenant creation/invitation screens) in the Pizzeria sample - only the domain/data/API plumbing needed to support a tenant-scoped `User`.
- Implementing full `ITenantRequest`/`TenantLoggingBehavior` wiring for every existing Pizzeria command/query (e.g. scoping `Order`/`Pizza` queries by tenant) - that is tracked as separate follow-up work under the parent issue #541, not this plan.
- Multi-tenant data isolation strategies beyond a shared-database/shared-schema-per-concern model (e.g. database-per-tenant) - out of scope for this iteration.
- Deciding the long-term shape of `Lewee.Configuration` (the proposed evolution of `Pizzeria.Configuration`) beyond what's needed to migrate/seed `AuthDbContext`; a full generic multi-DbContext configuration host is a candidate for a separate change.

## Decisions

1. **`Tenant` is the aggregate root; `User` is a child entity, not a second aggregate root.**
   `Tenant : AggregateRoot` in `Lewee.Auth.Domain` owns a `Users` collection. `User` becomes a plain `Entity` (still deriving from `Lewee.Domain.Entity` for `Id`/audit fields/soft-delete, but no longer `AggregateRoot`), constructed only via `Tenant.CreateUser(externalId, correlationId)` (or equivalent factory method on `Tenant`), which raises `UserCreatedEvent` from the tenant.
   *Alternative considered*: Keep `User` as its own aggregate root with a `TenantId` foreign key only (no navigation/ownership from `Tenant`). Rejected because the issue explicitly states "`User` will no longer be the aggregate root, instead `Tenant` will be" and because uniqueness/consistency rules (unique external ID per tenant) are naturally enforced within a single aggregate boundary.

2. **New `Lewee.Auth.Domain` project houses `Tenant`, `User`, `UserByExternalIdSpecification`, and the domain events.**
   Mirrors the existing layering: `Lewee.Auth.Domain` references only `Lewee.Domain`, matching how `Pizzeria.Store.Domain` currently references `Lewee.Domain`. `UserByExternalIdSpecification` is updated to filter by both `TenantId` and `ExternalId` (see spec `auth/tenant-management` - "Tenant-scoped queries exclude other tenants' data").
   *Alternative considered*: Keep `User`/`Tenant` in a single `Lewee.Auth` project without further layering (no separate Domain/Application/Infrastructure/Api split). Rejected because it would break the established Lewee convention of one project per architectural layer (see `repository-structure.instructions.md`), and because issue #541 explicitly requests the four-project split.

3. **`AuthDbContext` lives in `Lewee.Auth.Infrastructure.Data` and owns its own EF Core migrations.**
   `AuthDbContext : ApplicationDbContext<AuthDbContext>` with `Schema => "auth"`, exposing `DbSet<Tenant> Tenants` (the `Users` navigation is reached via `Tenant.Users`, not a top-level `DbSet<User>`, keeping with the aggregate boundary). `TenantConfiguration : AggregateRootConfiguration<Tenant>` configures the `Users` collection as an owned/dependent collection with a shadow or explicit `TenantId` foreign key; a `UserConfiguration : EntityConfiguration<User>` (not `AggregateRootConfiguration`, since `User` is no longer an aggregate root) configures the `ExternalId` unique index scoped to `(TenantId, ExternalId)` instead of a global unique index on `ExternalId` alone.
   *Alternative considered*: Reuse `StoreDbContext` and simply add a `Tenants` `DbSet` alongside the existing `Users` `DbSet`. Rejected because it perpetuates the coupling this change is meant to remove (auth data living in an application-specific context/schema) and contradicts the explicit ask for a new `auth` schema.

4. **Migration strategy: additive migration first, then a data-migration step, then a removal migration.**
   Three EF artifacts are introduced in sequence: (a) an initial `Lewee.Auth.Infrastructure.Data` migration that creates the `auth` schema with `Tenants`/`Users` tables; (b) a one-time data migration (either a raw-SQL `migrationBuilder.Sql(...)` step in an EF migration, or a dedicated seeding/migration routine invoked from the configuration host) that creates a default `Tenant` for existing data and copies each `sto.Users` row into `auth.Users` with that `TenantId`; (c) a `Pizzeria.Store.Data` migration that drops the `Users` table from `sto` and removes the `DbSet<User>`/`UserConfiguration` from `StoreDbContext`. Ordering matters: (a) and (b) must be applied (in that order) before (c), so the data has somewhere to land before the source table is dropped.
   *Alternative considered*: A single combined migration spanning both `AuthDbContext` and `StoreDbContext`. Rejected because EF Core migrations are scoped to a single `DbContext`; cross-context data movement must be orchestrated by application code (e.g. the configuration host), not a single migration file.

5. **Database migration/seeding host gains a step for `AuthDbContext`, without redesigning `Pizzeria.Configuration` wholesale in this change.**
   `Pizzeria.Configuration`'s `Program.cs` registers `AuthDbContext` alongside `StoreDbContext` and calls `serviceProvider.MigrateDatabaseAsync<AuthDbContext>(seedData: true, ...)` before the `StoreDbContext` migration that removes `sto.Users` is applied, and a new `IDatabaseSeeder<AuthDbContext>` seeds the default/administrative `Tenant` + `User` (external ID initially set from configuration/environment, then reconciled with the actual Keycloak user ID once available). The broader idea from issue #541 of renaming/generalizing `Pizzeria.Configuration` into a `Lewee.Configuration` that scans dependencies for `ApplicationDbContext` types is noted as a valuable follow-up but is explicitly deferred (see Non-Goals) to avoid conflating two large changes.
   *Alternative considered*: Build the generalized `Lewee.Configuration` auto-discovery host as part of this change. Rejected to keep this change's blast radius focused on introducing `Tenant`/`auth` schema; auto-discovery is an orthogonal capability that can be layered on afterward without affecting the domain/data model decided here.

6. **`CreateUserCommand`/`CreateUserEndpoint` replace `CreateCustomerCommand`/`CreateCustomerEndpoint`, gaining a `TenantId`.**
   `CreateUserCommand(string ExternalUserId, Guid TenantId) : ICommand` lives in `Lewee.Auth.Application`; its handler loads the `Tenant` aggregate (via `IRepository<Tenant>`), checks `UserByExternalIdSpecification` scoped to that tenant for idempotency, and calls `tenant.CreateUser(externalId, correlationId)` before saving. `CreateUserEndpoint : CommandEndpoint<CreateUserRequest>` in `Lewee.Auth.Api` remains anonymous-access, matching current `CreateCustomerEndpoint` behavior, since it fires during the OpenID Connect handshake before an application session exists. Until tenant selection/resolution is designed (Non-Goals), the endpoint resolves `TenantId` from a single seeded default tenant (e.g. via configuration or a well-known lookup), which is sufficient for the single-tenant sample today and can be replaced by real tenant resolution in a later change without altering the command's shape.
   *Alternative considered*: Infer `TenantId` from a claim already present on the Keycloak token (e.g. a custom `tid` claim). Rejected for this iteration because Keycloak realm/client configuration for a `tid` claim is not yet defined; documented as a natural evolution once tenant selection is designed.

## Risks / Trade-offs

- [Moving `User` from an aggregate root to a child entity is a significant EF Core mapping change (removing a top-level `DbSet<User>`, remapping the foreign key/ownership) and could be error-prone to migrate safely] → Mitigate with the staged migration plan (Decision 4) and by adding integration/unit test coverage asserting existing users survive migration with correct `TenantId` associations before merging the removal migration.
- [Single "default tenant" resolution for `CreateUserEndpoint` is a placeholder, not real multi-tenant routing] → Documented as a Non-Goal; the command/endpoint shape (`TenantId` as an explicit parameter) is designed so a future change can swap in real tenant resolution (e.g. from a subdomain, header, or JWT claim) without changing `Lewee.Auth.Application`/`Lewee.Auth.Domain`.
- [Splitting `Pizzeria.Configuration` migration/seeding logic across two `DbContext`s increases startup complexity and ordering risk (auth migration/seed must precede the `sto.Users`-dropping migration)] → Mitigate by sequencing `MigrateDatabaseAsync<AuthDbContext>` before `MigrateDatabaseAsync<StoreDbContext>` in `Pizzeria.Configuration`'s `Program.cs`/`PizzeriaStoreDatabaseConfigurationService`, and by adding a test/verification step confirming both contexts migrate cleanly in order against a fresh database.
- [Renaming `CreateCustomerCommand`/`CreateCustomerEndpoint`/`CreateCustomerRequest` to `CreateUser*` is a breaking rename for anything referencing the old names/routes] → Acceptable pre-1.0; called out as **BREAKING** in `proposal.md`.

## Migration Plan

1. Create `Lewee.Auth.Domain` with `Tenant`, `User`, `UserByExternalIdSpecification`, `UserCreatedEvent` (moved/adapted from `Pizzeria.Store.Domain`), add project to `lewee.slnx`.
2. Create `Lewee.Auth.Infrastructure.Data` with `AuthDbContext`, `TenantConfiguration`, `UserConfiguration`, and the initial `auth`-schema migration; add project to `lewee.slnx`.
3. Create `Lewee.Auth.Application` with `CreateUserCommand` (+ validator/handler), moved/renamed from `Pizzeria.Store.Application.Customers.CreateCustomerCommand`; add project to `lewee.slnx`.
4. Create `Lewee.Auth.Api` with `CreateUserEndpoint`, moved/renamed from `Pizzeria.Store.Api.Customers.CreateCustomerEndpoint`; add project to `lewee.slnx`; add a project reference from `Pizzeria.Store.Api` to `Lewee.Auth.Api` (and `Lewee.Auth.Application`/`Lewee.Auth.Infrastructure.Data` as needed for DI registration).
5. Update `Pizzeria.Configuration` to register/migrate/seed `AuthDbContext` (with a default `Tenant` + administrative `User`) ahead of `StoreDbContext`.
6. Add the data-migration step (Decision 4b) that copies existing `sto.Users` rows into `auth.Users` under a default `Tenant`.
7. Remove `User`, `UserByExternalIdSpecification`, `UserCreatedEvent`, `UserConfiguration` from `Pizzeria.Store.Domain`/`Pizzeria.Store.Data`; remove the `Users` `DbSet` from `StoreDbContext`; add the `Pizzeria.Store.Data` migration dropping `sto.Users` (Decision 4c).
8. Remove `CreateCustomerCommand`/`CreateCustomerEndpoint`/`Customers` folders from `Pizzeria.Store.Application`/`Pizzeria.Store.Api`; update `Pizzeria.Store.Contracts.Users` DTOs as needed.
9. Update `Pizzeria.Store.Web`'s `TokenValidatedContextExtensions`/`IStoreApiClient` to call the new create-user endpoint/route.
10. Add/port unit tests: `Lewee.Auth.Domain.Tests.Unit`, `Lewee.Auth.Application.Tests.Unit` (porting the existing `CreateCustomerCommand` test coverage referenced in `sample-tests/AGENTS.md` to `CreateUserCommand`), and update any `Pizzeria.Store.*.Tests.*` that referenced the removed types.
11. Run `dotnet build --configuration Release --nologo`, `dotnet test --filter "FullyQualifiedName!~Integration" --configuration Release --no-build --nologo`, and `dotnet format` to confirm no regressions.
12. Run the `Pizzeria.Tests.Integration` suite (Aspire-tagged, per repository convention) locally/manually to confirm the migrated database and end-to-end first-login flow behave as expected, since this suite is excluded from CI per issue #505.

No automated rollback is planned for the schema/data migration beyond standard EF Core migration `Down()` methods; because this changes a production data model (moving rows between schemas), a database backup before applying migrations in any real environment is recommended, consistent with standard practice for destructive migrations.

## Open Questions

- Exact mechanism for resolving `TenantId` in `CreateUserEndpoint` for the sample app (configuration-based default tenant vs. a lookup keyed by realm) can be decided during implementation without affecting the command/domain shape - both options satisfy the `auth/user-provisioning` spec.
- Whether the data-migration step (Decision 4b) is expressed as raw SQL inside an EF migration or as an idempotent one-time routine run by `Pizzeria.Configuration` can be decided during implementation; both satisfy the `auth/data-persistence` spec's requirement that existing users are preserved.
