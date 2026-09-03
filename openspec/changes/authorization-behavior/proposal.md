## Why

Issue #87 identifies two authorization gaps in the `MediatR` pipeline that `Lewee.Application` does not yet address: (1) restricting some commands/queries to a site administrator irrespective of tenant, and (2) restricting others to callers who hold a specific role within the tenant the request pertains to. Today `Lewee.Application.Mediation.Behaviors` only provides logging, validation, and exception-handling behaviors, and `Lewee.Auth.Domain` has no concept of a site administrator or a tenant role, so neither authorization check is currently possible even at the domain level. Per issue #528, this change captures the **OpenSpec implementation plan only** (proposal, design, specs, tasks) for closing this gap - no production code is added or modified here; a follow-up change implements the tasks tracked in `tasks.md`.

## What Changes

- Add `IsSiteAdministrator` (a plain boolean) to `User` in `Lewee.Auth.Domain`: identifies the small, fixed set of system-wide site administrators, independent of any tenant or role. No application command manages this flag - given how rarely it changes, it is set directly against the database (e.g. a SQL `UPDATE`).
- Remove the reserved `"ADMIN"` tenant seeded by `sample/Pizzeria.Configuration/AuthSeeder.cs`: it was mistakenly introduced in a previous PR to model site administration as tenant membership, and is no longer needed now that site administration is a direct flag on `User`.
- Add role assignment to `Lewee.Auth.Domain`: a new global `Role` entity (`Id` primary key, a globally-unique `Code`, and a `Name` that is not required to be unique), defined by a site administrator (not owned or defined per `Tenant`), that any `Tenant` can assign to its own members - a `TenantMembership` can be assigned zero, one, or many roles (e.g. a Pizzeria-defined "Manager", "Store Worker", or "Delivery Driver"). Because `Role` is a shared, global catalog rather than something each tenant defines for itself, no cross-tenant validation is needed when assigning it.
- Add a fast, non-relational-join lookup (a stored query/read model keyed by external user ID + tenant ID, resolving to the caller's assigned role codes for that tenant) in `Lewee.Auth.Infrastructure.Data`/`Lewee.Auth.Application`, kept in sync by domain event handlers whenever tenant membership or role assignments change.
- Add `IAdministratorRequest` (an empty marker interface) and `ITenantRoleRequest` (extending the existing `Lewee.Application.Mediation.Requests.ITenantRequest`, exposing the set of roles that satisfy the request - the caller must hold at least one) to **`Lewee.Auth.Application`**, not `Lewee.Application`, since both are specific to applications that adopt `Lewee.Auth.*`.
- Add `AdministratorAuthorizationBehavior<TRequest, TResponse>` and `TenantRoleAuthorizationBehavior<TRequest, TResponse>` to **`Lewee.Auth.Application`**: `IPipelineBehavior`s constrained to `IAdministratorRequest`/`ITenantRoleRequest` respectively. `AdministratorAuthorizationBehavior` resolves the caller and checks `User.IsSiteAdministrator` directly - no tenant or role lookup involved. `TenantRoleAuthorizationBehavior` resolves the caller, consults the authorization lookup, and checks tenant membership plus at least one of the request's required roles. Both short-circuit with a `CommandResult`/`QueryResult` failure (`ResultStatus.Unauthenticated` when no caller is resolved, `ResultStatus.Unauthorized` when the check fails - already mapped to 401/403 by `CommandEndpoint`/`QueryEndpoint`) rather than invoking the handler.
- Move `TenantLoggingBehavior` from `Lewee.Application` to `Lewee.Auth.Application`: it only makes sense for hosts using `Lewee.Auth.*` tenant membership, and should have moved there when `Lewee.Auth.*` was introduced (a gap missed in the `multi-tenancy-support` change).
- Add `ApplicationAuthConfiguration` to `Lewee.Auth.Application`: a new, opt-in registration point (distinct from the existing `AuthApplicationConfiguration`, which registers MediatR handlers/validators) that registers `TenantLoggingBehavior`, `AdministratorAuthorizationBehavior`, and `TenantRoleAuthorizationBehavior` as pipeline behaviors, via `Lewee.Application.ApplicationConfiguration.AddPipelineBehaviors`'s existing `additionalBehaviors` parameter. This is opt-in because not every consumer of `Lewee.Auth.Application` requires these authorization behaviors.

## Capabilities

### New Capabilities

- `auth/role-management`: Defines how a site administrator defines a global catalog of `Role`s, and how they are assigned to/removed from a `User`'s `TenantMembership`, with a membership able to hold multiple roles concurrently.
- `auth/authorization-lookup`: Defines the fast, tenant+user-keyed read model that resolves a caller's roles for a tenant, and how it stays in sync with `auth/role-management` and `auth/tenant-management` domain events.
- `application/administrator-authorization`: Defines `IAdministratorRequest` and `AdministratorAuthorizationBehavior`, authorizing site-administrator-only commands/queries by checking `User.IsSiteAdministrator` directly.
- `application/tenant-role-authorization`: Defines `ITenantRoleRequest` and `TenantRoleAuthorizationBehavior`, authorizing commands/queries that require tenant membership plus at least one of a set of roles within `request.TenantId`.

### Modified Capabilities

- None. `auth/tenant-management`, `auth/user-provisioning`, and `auth/data-persistence` (proposed by `openspec/changes/multi-tenancy-support`, not yet archived into `openspec/specs/`) are extended by, but not changed in their existing requirements by, this plan - `TenantMembership`'s existing creation/removal/idempotency behavior is unaffected; `User.IsSiteAdministrator` and `Role` are additive.

## Impact

- **Affected code (framework, `src/`)**:
  - `src/Lewee.Auth.Domain`: `User` gains `IsSiteAdministrator`; a new `Role` root entity (globally-unique code, not owned by `Tenant`); `TenantMembership` gains role assignment(s); new domain events (e.g. role defined, role assigned/removed).
  - `src/Lewee.Auth.Infrastructure.Data`: EF configuration/migration for `IsSiteAdministrator`, the new `Role` entity, and role assignments, plus the new authorization lookup read model/table and its maintenance.
  - `src/Lewee.Auth.Application`: gains `IAdministratorRequest`, `ITenantRoleRequest`, `AdministratorAuthorizationBehavior`, `TenantRoleAuthorizationBehavior`, `TenantLoggingBehavior` (moved from `Lewee.Application`), domain event handlers that maintain the authorization lookup, and a new `ApplicationAuthConfiguration` class that registers all of the above as opt-in pipeline behaviors.
  - `src/Lewee.Application`: `TenantLoggingBehavior` is removed from `Lewee.Application.Mediation.Behaviors` (moved to `Lewee.Auth.Application`, see above); otherwise unchanged - no new marker interfaces or behaviors are added here.
  - `sample/Pizzeria.Configuration/AuthSeeder.cs`: the reserved `"ADMIN"` tenant creation is removed; the seeded admin user's `IsSiteAdministrator` flag is set directly instead (no data migration - the sample's database can be recreated).
- **Affected code (sample, `sample/`)**: `AuthSeeder.cs` change above; otherwise none required by this plan-only change. `design.md` notes how `Pizzeria.*` commands/queries would later opt in to `IAdministratorRequest`/`ITenantRoleRequest` as a follow-up.
- **Dependencies**: no new external dependencies; reuses existing `MediatR`, `Lewee.Common` (`IAuthenticatedUserService`, `Result`/`ResultStatus`), and `Lewee.Domain`/`Lewee.Infrastructure.Data` patterns.
- **Breaking change**: removing `TenantLoggingBehavior` from `Lewee.Application` is source-breaking for any host that references it from that namespace directly (rather than via `ApplicationConfiguration.AddPipelineBehaviors`); not currently referenced directly outside the framework. Both new pipeline behaviors are opt-in via marker interfaces and `ApplicationAuthConfiguration`; existing commands/queries that do not implement `IAdministratorRequest`/`ITenantRoleRequest` are unaffected.

## Note

This change captures the **implementation plan only** (proposal, design, specs, tasks), per issue #528. No production code is modified as part of this change; `tasks.md` tracks the follow-up implementation work under parent issue #87.
