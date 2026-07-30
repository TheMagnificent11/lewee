## Context

See `proposal.md` for motivation. Relevant current state:

- MediatR pipeline behaviors already exist as a composable pattern in `Lewee.Application/Mediation/Behaviors/` (`TenantLoggingBehavior<TRequest, TResponse>` constrains itself to `TRequest : ITenantRequest`, and behaviors are registered centrally in `ApplicationConfiguration.AddPipelineBehaviors`). Authorization behaviors follow the same shape: an opt-in marker/data interface on the request, plus a generic pipeline behavior constrained to that interface.
- `ICommand` returns `CommandResult` and `IQuery<T>` returns `QueryResult<T>` (both in `Lewee.Common`); short-circuiting a request must produce a failure `CommandResult`/`QueryResult` rather than throwing, consistent with `ValidationBehavior`'s and `DomainExceptionBehavior`'s approach of returning failure results instead of exceptions for expected rejection paths.
- `Lewee.Infrastructure.Auth.IAuthenticatedUserService` already exposes the current caller's external user ID (from `ClaimTypes.NameIdentifier` via `IHttpContextAccessor`), but nothing today maps that external ID to an application user, site-administrator flag, or tenant roles.
- `Pizzeria.Store.Domain.User` (sample-only) is the only existing "user" concept, stored via `Pizzeria.Store.Data.StoreDbContext` in the `sto` schema; it has no administrator flag and no role/tenant association.
- `Lewee.Domain` must remain free of dependencies on other Lewee layers or infrastructure packages (clean architecture boundary, per `decision-making.instructions.md`).

## Goals / Non-Goals

**Goals:**
- Define a `Lewee.Auth` package family that owns the `User` aggregate, administrator status, and tenant role assignments as first-class framework concepts (not sample-app-specific).
- Define two independent, composable MediatR pipeline behaviors — administrator authorization and tenant-role authorization — that plug into the existing `AddPipelineBehaviors` registration without disturbing existing behaviors' ordering guarantees (validation, exception handling, logging).
- Keep role/administrator lookups fast (target: single indexed read, not a chain of joins) by maintaining a denormalized read model updated via domain event handlers, rather than performing complex authorization joins inline on every request.
- Keep `Lewee.Auth.Domain` dependent only on `Lewee.Domain`, and keep the pipeline behaviors in `Lewee.Application` free of any direct Keycloak/ASP.NET Core dependency (they depend on `Lewee.Auth.Application`-level abstractions instead).

**Non-Goals:**
- Building a full RBAC administration UI in this change (API endpoints for role CRUD are in scope; a Blazor admin screen is not).
- Changing how Keycloak issues tokens or adding claims to Keycloak tokens — claims-in-token authorization is explicitly rejected by issue #87 in favor of database-backed roles.
- Multi-level role hierarchies or permission composition beyond "does the user hold role X for tenant Y" — a single flat role-per-tenant-per-user model is sufficient for this iteration.
- Migrating existing `Pizzeria.Store.Application.Customers.CreateCustomerCommand` callers other than the Keycloak `OnTokenValidated` wiring identified in the proposal.

## Decisions

1. **Two separate marker/data interfaces (`IAdministratorRequest` and a tenant-role request interface) rather than one combined interface.**
   Administrator checks are tenant-independent (a super admin bypasses tenant scoping entirely), while tenant-role checks require `ITenantRequest` (an existing interface) to already be present on the same request to know which tenant to check roles against. Keeping them separate lets a command require *either* administrator access *or* a tenant role (or both, or neither) without an awkward combined contract.
   *Alternative considered*: A single `IAuthorizedRequest` with an enum discriminator (`Administrator` vs `TenantRole`). Rejected because it couples two independent authorization concerns into one type and complicates the "requires admin OR one of several roles" case; separate interfaces compose more naturally as independent pipeline behaviors, matching the existing `TenantLoggingBehavior` pattern of one interface → one behavior.

2. **New `Lewee.Auth.*` package family instead of extending `Lewee.Infrastructure.Auth`.**
   `Lewee.Infrastructure.Auth` currently only exposes `IAuthenticatedUserService` (a thin read-only wrapper over `HttpContext.User`), with no persistence. User/role data needs Domain, Application, Infrastructure.Data, and Api layers of its own — mirroring how `Pizzeria.Store.*` is structured — so `Lewee.Auth` is introduced as its own vertical slice (`Lewee.Auth.Domain`, `Lewee.Auth.Infrastructure.Data`, `Lewee.Auth.Application`, `Lewee.Auth.Api`) rather than overloading the existing thin `Lewee.Infrastructure.Auth` package.
   *Alternative considered*: Keep `User` in each sample/consuming application and only ship the pipeline behaviors + interfaces from `Lewee.Application`, leaving role storage entirely up to each consumer. Rejected because issue #87 explicitly asks for `User` to become a Lewee framework concept (via a new `auth` schema) so authorization is consistent across any application built on Lewee, not re-implemented per sample.

3. **Role lookups resolved through an `Lewee.Auth.Application`-owned service abstraction injected into the pipeline behavior, not through direct DB access from `Lewee.Application`.**
   `TenantRoleAuthorizationBehavior` (living in `Lewee.Application` alongside the other generic behaviors) depends on an interface such as `ICallerAuthorizationContext` (or similarly named), implemented in `Lewee.Auth.Infrastructure.Data`, that answers "is the caller a site administrator?" and "does the caller hold role R for tenant T?". This keeps `Lewee.Application` free of a hard dependency on EF Core/`Lewee.Auth.Infrastructure.Data`, matching the existing pattern where `Lewee.Application` depends only on abstractions (e.g., `ICorrelationContextAccessor` from `Correlate`, not a concrete implementation).
   *Alternative considered*: Have the behavior call `Lewee.Infrastructure.Auth.IAuthenticatedUserService` directly and perform the DB lookup inline in `Lewee.Application`. Rejected because `Lewee.Application` should not take a compile-time dependency on a specific persistence technology; the abstraction is defined where the behavior lives and implemented where the data access happens, consistent with clean architecture dependency inversion.

4. **Denormalized, event-maintained role lookup rather than a live join on every request.**
   A read-optimized table/query (keyed by `TenantId` + `UserId`, returning the set of roles) is updated by domain event handlers reacting to role-assignment changes (grant/revoke), as suggested in issue #87. This keeps the hot authorization-check path to a single indexed lookup instead of joining users, tenant memberships, and role tables on every command/query.
   *Alternative considered*: Query the normalized role-assignment tables directly on every request. Rejected for this design because issue #87 explicitly calls out wanting "a stored query with an appropriate key... fast SQL query"; the extra complexity of keeping a projection in sync is accepted as a deliberate performance trade-off (see Risks).

5. **`AuthDbContext` and its migration live in `Lewee.Auth.Infrastructure.Data`, following the existing `Lewee.Infrastructure.Data`/EF Core conventions used by `Pizzeria.Store.Data`.**
   `Lewee.Auth.Infrastructure.Data` references `Lewee.Infrastructure.Data` (for shared `ApplicationDbContext`-style base classes) and defines the `auth` schema. The relocation migration is a two-step data migration: (a) create `auth.Users` and copy data from `sto.Users`, (b) drop `sto.Users` once `Pizzeria.Store.Data.StoreDbContext` no longer references it.
   *Alternative considered*: Keep `Users` in `StoreDbContext`'s `sto` schema and only add roles in a new schema. Rejected because issue #87 explicitly directs that `User` move to a new `auth` schema owned by the new `Lewee.Auth` package, not remain sample-app-owned.

6. **Administrator/user provisioning at startup is handled by extending the existing migration/seeding host (`Pizzeria.Configuration`) rather than building new tooling in this change.**
   The existing `Pizzeria.Configuration` console app (registered via Aspire's `AddCSharpApp`, per repository conventions) is extended to also migrate `AuthDbContext` and seed the initial administrator user (creating the corresponding Keycloak identity if absent, then recording its external ID). A future rename to `Lewee.Configuration` (as speculated in issue #87) is treated as a follow-up, not required for this change to land.
   *Alternative considered*: Build a brand-new generic "migrate any `ApplicationDbContext` found via assembly scanning" host now. Deferred as a non-goal for this iteration to keep the change scoped to authorization behavior; `Pizzeria.Configuration` is extended directly for `AuthDbContext` instead.

## Risks / Trade-offs

- [Denormalized role-lookup projection can drift from the source-of-truth role assignments if an event handler fails or is missed] → Mitigate by making the projection rebuildable from source tables (an idempotent rebuild path) and by covering the domain-event-driven update path with unit/integration tests asserting the projection reflects grant/revoke changes.
- [Moving `sto.Users` to `auth.Users` is a breaking, data-carrying migration] → Mitigate by writing the migration as an explicit copy-then-drop (not a rename-in-place) so it can be reviewed and, if needed, rolled back before the drop step is applied; document the migration clearly in `tasks.md` and require it to run before the old `StoreDbContext.Users` DbSet is removed from code.
- [Two new pipeline behaviors add per-request latency for authorization-checked requests] → Mitigate by keeping the tenant-role check to a single indexed lookup (per Decision 4) and by only registering/executing the behaviors for requests that actually implement the relevant marker interfaces (MediatR's generic constraint mechanism already ensures behaviors constrained to `IAdministratorRequest`/tenant-role interfaces are skipped entirely for requests that don't implement them).
- [Introducing a new package family (`Lewee.Auth.*`) increases the framework's surface area and solution complexity] → Accepted as necessary per issue #87's explicit request; scoped tightly to user identity, administrator flag, and tenant roles rather than broader user-profile concerns.
- [Bootstrapping the first administrator user requires coordinating with Keycloak, which may not be reachable at startup in some environments] → Mitigate by making the seeding step idempotent and tolerant of "administrator already exists" (skip), consistent with existing `DatabaseConfiguration.MigrateDatabaseAsync` idempotency patterns.

## Migration Plan

1. Scaffold `Lewee.Auth.Domain`, `Lewee.Auth.Infrastructure.Data`, `Lewee.Auth.Application`, `Lewee.Auth.Api` projects (and matching `tests/Lewee.Auth.*.Tests.Unit` projects), added to `lewee.slnx`.
2. Move `User`, `UserByExternalIdSpecification`, `UserCreatedEvent` from `Pizzeria.Store.Domain` into `Lewee.Auth.Domain`; add an administrator flag and tenant-role association to the `User`/related aggregates.
3. Create `AuthDbContext` in `Lewee.Auth.Infrastructure.Data` with the `auth` schema; move `UserConfiguration`; add configuration for role assignments and the denormalized role-lookup projection.
4. Write the EF Core migration that creates `auth.Users` (and new role tables), copies existing `sto.Users` rows across, and (in a follow-up migration once code no longer references it) drops `sto.Users`.
5. Add `IAdministratorRequest` and the tenant-role request interface to `Lewee.Application/Mediation/Requests/`; add `AdministratorAuthorizationBehavior` and `TenantRoleAuthorizationBehavior` to `Lewee.Application/Mediation/Behaviors/`; register both in `ApplicationConfiguration.AddPipelineBehaviors`.
6. Implement the caller-authorization abstraction and its `Lewee.Auth.Infrastructure.Data`-backed implementation, plus domain event handlers that keep the role-lookup projection current.
7. Migrate `CreateCustomerCommand`/`CreateCustomerEndpoint` to `CreateUserCommand`/`CreateUserEndpoint` in `Lewee.Auth.Application`/`Lewee.Auth.Api`; update `Pizzeria.Store.Api` to reference `Lewee.Auth.Api`; update `Pizzeria.Store.Web`'s `OnTokenValidated` handler.
8. Add role-management API endpoints (create role, assign role, revoke role) in `Lewee.Auth.Api`.
9. Extend `Pizzeria.Configuration` to migrate `AuthDbContext` and seed the initial administrator user/Keycloak identity.
10. Update all affected unit/integration tests; run `dotnet build --configuration Release --nologo`, `dotnet test --filter "FullyQualifiedName!~Integration" --configuration Release --no-build --nologo`, and `dotnet format` to confirm no regressions.

No automated rollback is planned for the schema migration beyond standard EF Core migration `Down()` methods; because this is pre-1.0 (per `technology-stack.instructions.md`), a forward-only migration with a reviewed, explicit copy-then-drop is considered acceptable.

## Open Questions

- Exact naming of the tenant-role request interface (e.g., `ITenantRoleRequest` vs `IRequiresTenantRole`) and the caller-authorization abstraction (e.g., `ICallerAuthorizationContext`) is left to implementation; naming should follow existing `I*Request`/`I*Service` conventions and can be finalized during task execution without affecting the specs above.
- Whether `Pizzeria.Configuration` is renamed to `Lewee.Configuration` as part of this change or deferred to a separate change is left open; the specs and behaviors in this design do not depend on that rename happening now.
