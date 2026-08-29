## Why

Issue #87 identifies two authorization gaps in the `MediatR` pipeline that `Lewee.Application` does not yet address: (1) restricting some commands/queries to a site administrator irrespective of tenant, and (2) restricting others to callers who hold a specific role within the tenant the request pertains to. Today `Lewee.Application.Mediation.Behaviors` only provides logging, validation, and exception-handling behaviors (`TenantLoggingBehavior` merely adds the tenant ID to the logging scope; it performs no authorization check), and `Lewee.Auth.Domain`'s `TenantMembership` has no concept of a role, so neither authorization check is currently possible even at the domain level. Per issue #528, this change captures the **OpenSpec implementation plan only** (proposal, design, specs, tasks) for closing this gap - no production code is added or modified here; a follow-up change implements the tasks tracked in `tasks.md`.

## What Changes

- Add a tenant-scoped `Role` concept to `Lewee.Auth.Domain`: roles are defined per `Tenant` (a tenant administrator can define its own roles) and assigned to a `User`'s `TenantMembership`, with domain events raised on role definition and assignment/removal.
- Add a fast, non-relational-join lookup (a stored query/read model keyed by external user ID + tenant ID, resolving to the caller's role codes for that tenant) in `Lewee.Auth.Infrastructure.Data`, kept in sync by domain event handlers whenever tenant membership or role assignments change, so the pipeline behaviors below avoid an expensive join per request.
- Add `IAdministratorRequest` to `Lewee.Application.Mediation.Requests`: a marker interface (exposing the required role) that a command/query implements to restrict it to a site administrator, irrespective of the tenant the request pertains to. Site administration is modeled as membership (with the required role) of the existing reserved "ADMIN" tenant (see `sample/Pizzeria.Configuration/AuthSeeder.cs`), so no separate site-admin concept is needed beyond the tenant/role model above.
- Add `ITenantRoleRequest` (extending the existing `ITenantRequest`) to `Lewee.Application.Mediation.Requests`: a marker interface (exposing the required role) that a command/query implements to restrict it to callers who are members of `TenantId` and hold the required role within that tenant.
- Add `AdministratorAuthorizationBehavior<TRequest, TResponse>` and `TenantRoleAuthorizationBehavior<TRequest, TResponse>` to `Lewee.Application.Mediation.Behaviors`: `IPipelineBehavior`s constrained to `IAdministratorRequest`/`ITenantRoleRequest` respectively, which resolve the current caller from `IAuthenticatedUserService`, consult the authorization lookup, and short-circuit the pipeline with a `CommandResult`/`QueryResult` failure (`ResultStatus.Unauthenticated` when no caller is resolved, `ResultStatus.Unauthorized` when the caller lacks the required membership/role - already mapped to 401/403 by `CommandEndpoint`/`QueryEndpoint`) rather than invoking the handler.
- Register both new behaviors in `ApplicationConfiguration.AddPipelineBehaviors`, defining their order relative to the existing behaviors (validation must still run first for well-formed requests; authorization must run before the handler and before performance/logging behaviors that assume the request is authorized).

## Capabilities

### New Capabilities
- `auth/role-management`: Defines the tenant-scoped `Role` concept and how roles are assigned to/removed from a `User`'s `TenantMembership`.
- `auth/authorization-lookup`: Defines the fast, tenant+user-keyed read model that resolves a caller's roles for a tenant, and how it stays in sync with `auth/role-management` and `auth/tenant-management` domain events.
- `application/administrator-authorization`: Defines `IAdministratorRequest` and `AdministratorAuthorizationBehavior`, authorizing site-administrator-only commands/queries via membership and role in the reserved "ADMIN" tenant.
- `application/tenant-role-authorization`: Defines `ITenantRoleRequest` and `TenantRoleAuthorizationBehavior`, authorizing commands/queries that require tenant membership plus a specific role within `request.TenantId`.

### Modified Capabilities
- None. `auth/tenant-management`, `auth/user-provisioning`, and `auth/data-persistence` (proposed by `openspec/changes/multi-tenancy-support`, not yet archived into `openspec/specs/`) are extended by, but not changed in their existing requirements by, this plan - `TenantMembership`'s existing creation/removal/idempotency behavior is unaffected; roles are additive.

## Impact

- **Affected code (framework, `src/`)**:
  - `src/Lewee.Auth.Domain`: new `Role` type (owned by `Tenant`), `TenantMembership` gains role assignment(s), new domain events (e.g. role defined, role assigned/removed).
  - `src/Lewee.Auth.Infrastructure.Data`: EF configuration/migration for roles and role assignments, plus the new authorization lookup read model/table and its maintenance (domain event handlers, likely in a new or existing `Lewee.Auth.Application` project).
  - `src/Lewee.Auth.Application`: domain event handlers that update the authorization lookup when `auth/role-management`/`auth/tenant-management` events occur.
  - `src/Lewee.Application/Mediation/Requests`: new `IAdministratorRequest`, `ITenantRoleRequest` (extends `ITenantRequest`).
  - `src/Lewee.Application/Mediation/Behaviors`: new `AdministratorAuthorizationBehavior`, `TenantRoleAuthorizationBehavior`; `ApplicationConfiguration.AddPipelineBehaviors` registers both.
- **Affected code (sample, `sample/`)**: none required by this plan-only change; `design.md` notes how `Pizzeria.*` commands/queries would later opt in to `IAdministratorRequest`/`ITenantRoleRequest` as a follow-up.
- **Dependencies**: no new external dependencies; reuses existing `MediatR`, `Lewee.Common` (`IAuthenticatedUserService`, `Result`/`ResultStatus`), and `Lewee.Domain`/`Lewee.Infrastructure.Data` patterns.
- **Breaking change**: none anticipated - both new pipeline behaviors are opt-in via marker interfaces; existing commands/queries that do not implement `IAdministratorRequest`/`ITenantRoleRequest` are unaffected.

## Note

This change captures the **implementation plan only** (proposal, design, specs, tasks), per issue #528. No production code is modified as part of this change; `tasks.md` tracks the follow-up implementation work under parent issue #87.
