## 1. Scaffolding

- [ ] 1.1 Create new C# projects: `src/Lewee.Auth.Domain`, `src/Lewee.Auth.Infrastructure.Data`, `src/Lewee.Auth.Application`, `src/Lewee.Auth.Api`, following existing project conventions in `src/Directory.Build.props`
- [ ] 1.2 Add the new projects to `lewee.slnx` and any required entries to `Directory.Packages.props`
- [ ] 1.3 Create matching unit test projects: `tests/Lewee.Auth.Domain.Tests.Unit`, `tests/Lewee.Auth.Application.Tests.Unit` (and infra tests if applicable), following `tests/Directory.Build.props` conventions

## 2. Domain Layer (Lewee.Auth.Domain)

- [ ] 2.1 Move `User`, `UserByExternalIdSpecification`, `UserCreatedEvent` from `sample/Pizzeria.Store.Domain` to `src/Lewee.Auth.Domain`, referencing `Lewee.Domain` only
- [ ] 2.2 Add a site-administrator flag/concept to the `User` aggregate (or a related domain concept) per `specs/administrator-authorization/spec.md` and `specs/user-management/spec.md`
- [ ] 2.3 Add a `TenantRole`/role-assignment domain concept (role definition, and a user-to-tenant-role assignment) per `specs/tenant-role-authorization/spec.md`, raising domain events on grant/revoke
- [ ] 2.4 Add unit tests for the moved/new domain classes in `tests/Lewee.Auth.Domain.Tests.Unit`

## 3. Infrastructure Data Layer (Lewee.Auth.Infrastructure.Data)

- [ ] 3.1 Create `AuthDbContext` in `src/Lewee.Auth.Infrastructure.Data`, referencing `Lewee.Infrastructure.Data`, using the `auth` schema
- [ ] 3.2 Move `UserConfiguration` from `sample/Pizzeria.Store.Data/Configuration` to `Lewee.Auth.Infrastructure.Data` and register it, along with configuration for the new role/role-assignment tables
- [ ] 3.3 Implement the denormalized tenant-role lookup projection (keyed by `TenantId` + `UserId`) and domain event handlers that update it when role assignments change
- [ ] 3.4 Implement the caller-authorization abstraction (administrator check + tenant-role lookup) defined in `design.md` Decision 3, backed by `AuthDbContext`
- [ ] 3.5 Author the EF Core migration that creates `auth.Users` and role tables and copies existing `sto.Users` data across; author a follow-up migration to drop `sto.Users` once no code references `StoreDbContext.Users`

## 4. Application Layer Contracts and Behaviors (Lewee.Application)

- [ ] 4.1 Add `IAdministratorRequest` to `src/Lewee.Application/Mediation/Requests/`
- [ ] 4.2 Add the tenant-role request interface (e.g. `ITenantRoleRequest`, extending/complementing `ITenantRequest`) to `src/Lewee.Application/Mediation/Requests/`
- [ ] 4.3 Add `AdministratorAuthorizationBehavior<TRequest, TResponse>` to `src/Lewee.Application/Mediation/Behaviors/`, constrained to `TRequest : IAdministratorRequest`, returning a failure `CommandResult`/`QueryResult` when the caller is not a site administrator, per `specs/administrator-authorization/spec.md`
- [ ] 4.4 Add `TenantRoleAuthorizationBehavior<TRequest, TResponse>` to `src/Lewee.Application/Mediation/Behaviors/`, constrained to the tenant-role request interface, returning a failure `CommandResult`/`QueryResult` when the caller lacks a required role for the request's tenant, per `specs/tenant-role-authorization/spec.md`
- [ ] 4.5 Register both new behaviors in `ApplicationConfiguration.AddPipelineBehaviors`, ordered appropriately relative to existing behaviors (e.g. after tenant/correlation logging, before performance/failure logging)
- [ ] 4.6 Add unit tests for both new behaviors in `tests/Lewee.Application.Tests.Unit` (e.g. `AdministratorAuthorizationBehaviorTests.cs`, `TenantRoleAuthorizationBehaviorTests.cs`), covering allow/deny/unauthenticated scenarios from the specs

## 5. Application Layer - User Management (Lewee.Auth.Application)

- [ ] 5.1 Move `CreateCustomerCommand` (and its log messages/handler) from `sample/Pizzeria.Store.Application/Customers` to `src/Lewee.Auth.Application`, renaming to `CreateUserCommand`
- [ ] 5.2 Move `CustomerCreatedEventHandler` (and log messages) to `Lewee.Auth.Application`, updating it to react to `UserCreatedEvent` in its new location
- [ ] 5.3 Add unit tests for `CreateUserCommand`/its handler in `tests/Lewee.Auth.Application.Tests.Unit`

## 6. API Layer (Lewee.Auth.Api)

- [ ] 6.1 Move `CreateCustomerEndpoint` from `sample/Pizzeria.Store.Api/Customers` to `src/Lewee.Auth.Api`, renaming to `CreateUserEndpoint`; move/rename corresponding contracts (`CreateCustomerRequest`, `CustomerDto`) from `sample/Pizzeria.Store.Contracts/Users`
- [ ] 6.2 Add a reference to `Lewee.Auth.Api` from `sample/Pizzeria.Store.Api`
- [ ] 6.3 Add role-management endpoints in `Lewee.Auth.Api`: create tenant role, assign role to user, revoke role from user, list a user's roles for a tenant
- [ ] 6.4 Add integration/unit tests for the new endpoints consistent with existing endpoint test patterns

## 7. Sample Application Wiring

- [ ] 7.1 Update `sample/Pizzeria.Store.Web`'s Keycloak `OnTokenValidated` handler to call the new `CreateUserEndpoint` (via `Lewee.Auth.Api`) instead of the old `CreateCustomerEndpoint`
- [ ] 7.2 Remove the now-unused `User`, `UserByExternalIdSpecification`, `UserCreatedEvent`, `UserConfiguration`, `CreateCustomerCommand`, `CreateCustomerEndpoint`, and related contracts from `sample/Pizzeria.Store.*` projects once superseded by their `Lewee.Auth.*` equivalents
- [ ] 7.3 Remove the `Users` DbSet from `Pizzeria.Store.Data.StoreDbContext` once `AuthDbContext` owns it, keeping the drop-`sto.Users` migration (task 3.5 follow-up) as a separate migration applied after this change

## 8. Startup Provisioning

- [ ] 8.1 Extend `sample/Pizzeria.Configuration` to migrate `AuthDbContext` alongside existing `ApplicationDbContext`-derived contexts
- [ ] 8.2 Add idempotent seeding logic that provisions an initial site-administrator user (creating a Keycloak identity if one does not already exist, then recording its external ID) per `specs/user-management/spec.md`

## 9. Verification

- [ ] 9.1 Search the repo for remaining references to the moved sample classes (`grep -r "Pizzeria.Store.Domain.User\|CreateCustomerCommand\|CreateCustomerEndpoint" src/ sample/ tests/ sample-tests/`) and update any missed call sites
- [ ] 9.2 Run `dotnet build --configuration Release --nologo` and confirm no errors or warnings
- [ ] 9.3 Run `dotnet test --filter "FullyQualifiedName!~Integration" --configuration Release --no-build --nologo` and confirm all unit tests pass
- [ ] 9.4 Run `dotnet format` to confirm code style compliance
- [ ] 9.5 Manually verify (via the sample app / integration tests) that an administrator-only command is rejected for a non-administrator and succeeds for an administrator, and that a tenant-role-restricted command is rejected/allowed based on role assignment, matching the scenarios in `specs/administrator-authorization/spec.md` and `specs/tenant-role-authorization/spec.md`
