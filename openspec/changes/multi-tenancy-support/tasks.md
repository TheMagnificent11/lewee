## 1. Lewee.Auth.Domain (new project)

- [x] 1.1 Create `src/Lewee.Auth.Domain` project referencing `Lewee.Domain` only, and add it to `lewee.slnx`
- [x] 1.2 Add `Tenant : AggregateRoot` with a `Name` (or equivalent) property, as its own independent aggregate root (no `Users` navigation/ownership)
- [x] 1.3 Move `User` from `sample/Pizzeria.Store.Domain/User.cs` into `Lewee.Auth.Domain`, keeping it an `AggregateRoot`; add a `TenantMemberships` collection navigation, an `AssignToTenant(tenantId, correlationId)` method that idempotently adds a `TenantMembership` and raises `TenantMembershipCreatedEvent`, and a `RemoveFromTenant(tenantId, correlationId)` method that idempotently removes the matching `TenantMembership` and raises `TenantMembershipRemovedEvent`. A newly-created `User` SHALL have zero memberships.
- [x] 1.4 Add `TenantMembership` as a child entity owned by `User`, holding a `TenantId` (and any audit fields consistent with `Lewee.Domain.Entity`)
- [x] 1.5 Move `UserCreatedEvent` from `sample/Pizzeria.Store.Domain/UserCreatedEvent.cs` into `Lewee.Auth.Domain` unchanged in shape; add `TenantMembershipCreatedEvent` (raised when a user is added to a tenant) and `TenantMembershipRemovedEvent` (raised when a user is removed from a tenant)
- [x] 1.6 Move `UserByExternalIdSpecification` from `sample/Pizzeria.Store.Domain/UserByExternalIdSpecification.cs` into `Lewee.Auth.Domain` unchanged (global external-ID lookup, not tenant-scoped)
- [x] 1.7 Add a `TenantCreatedEvent` (or reuse an existing pattern) raised when a `Tenant` is created
- [x] 1.8 Create `tests/Lewee.Auth.Domain.Tests.Unit` covering: tenant creation, user creation with zero memberships, assigning a user to a tenant (raises `TenantMembershipCreatedEvent`), idempotent re-assignment to the same tenant (no duplicate event), assigning the same user to multiple tenants, removing a user from a tenant (raises `TenantMembershipRemovedEvent`), and idempotent removal of a membership that does not exist (no event raised) - per `auth/tenant-management` spec scenarios

## 2. Lewee.Auth.Infrastructure.Data (new project)

- [x] 2.1 Create `src/Lewee.Auth.Infrastructure.Data` project referencing `Lewee.Infrastructure.Data` and `Lewee.Auth.Domain`, and add it to `lewee.slnx`
- [x] 2.2 Add `AuthDbContext : ApplicationDbContext<AuthDbContext>` with `Schema => "auth"`, `DbSet<Tenant> Tenants`, and `DbSet<User> Users`
- [x] 2.3 Add `TenantConfiguration : AggregateRootConfiguration<Tenant>` configuring `Tenant`'s own properties (no `Users` navigation)
- [x] 2.4 Add `UserConfiguration : AggregateRootConfiguration<User>` configuring the global unique index on `ExternalId` and the owned `TenantMemberships` collection, mapped to an `auth.UserTenantMemberships` table with a unique index on `(UserId, TenantId)` and a foreign key to `auth.Tenants`
- [x] 2.5 Add the initial EF Core migration creating the `auth` schema with `Tenants`/`Users`/`UserTenantMemberships` tables
- [x] 2.6 Create `tests/Lewee.Auth.Infrastructure.Data.Tests.Unit` covering the EF configuration (schema name, global unique index on `ExternalId`, unique membership index), following existing patterns in `tests/Lewee.Infrastructure.Data.Tests.Unit`

## 3. Lewee.Auth.Application (new project)

- [x] 3.1 Create `src/Lewee.Auth.Application` project referencing `Lewee.Application` and `Lewee.Auth.Domain`, and add it to `lewee.slnx`
- [x] 3.2 Add `CreateUserCommand(string ExternalUserId) : ICommand` with a `Validator` (non-empty `ExternalUserId` within `User.FieldLengths.ExternalId`) ported from `sample/Pizzeria.Store.Application/Customers/CreateCustomerCommand.cs`; the command SHALL NOT accept a `TenantId`
- [x] 3.3 Add the `CreateUserCommand.Handler`, using `UserByExternalIdSpecification` for idempotency and calling `User.Create(...)` before saving; the created `User` SHALL have no tenant membership
- [x] 3.4 Port `CustomerCreatedEventHandler`/`CustomerCreatedEventHandlerLogMessages`/`CreateCustomerCommandLogMessages` to `Lewee.Auth.Application`, renamed to match `UserCreatedEvent`/`CreateUserCommand`
- [x] 3.5 Create `tests/Lewee.Auth.Application.Tests.Unit`, porting the existing `CreateCustomerCommand` test coverage to `CreateUserCommand` (valid create with zero memberships, idempotent duplicate by external ID)
- [x] 3.6 (Deferred, tracked only, not implemented in this change) note the future need for an `AssignUserToTenantCommand` (or equivalent) to assign an existing `User` to a `Tenant` - see `design.md` Non-Goals/Open Questions

## 4. Lewee.Auth.Api (new project)

- [x] 4.1 Create `src/Lewee.Auth.Api` project referencing `Lewee.Infrastructure.FastEndpoints` and `Lewee.Auth.Application`, and add it to `lewee.slnx`
- [x] 4.2 Add `CreateUserRequest`/`UserDto` contracts (in `Lewee.Auth.Api` or a new `Lewee.Auth.Contracts` project, matching how `Pizzeria.Store.Contracts.Users` is structured today) with just `ExternalUserId` (no `TenantId`)
- [x] 4.3 Add `CreateUserEndpoint : CommandEndpoint<CreateUserRequest>` (anonymous access allowed) ported from `sample/Pizzeria.Store.Api/Customers/CreateCustomerEndpoint.cs`
- [x] 4.4 Add a project reference from `Pizzeria.Store.Api` to `Lewee.Auth.Api` (and `Lewee.Auth.Application`/`Lewee.Auth.Infrastructure.Data` for DI registration) so the endpoint is exposed by the sample API
- [x] 4.5 Register `AddLeweeDatabaseServices<AuthDbContext>(...)` and any `Lewee.Auth.Application`/`Lewee.Auth.Api` DI extension methods in `Pizzeria.Store.Api`'s `Program.cs`

## 5. Database Migration and Seeding

- [x] 5.1 Update `Pizzeria.Configuration` to register `AuthDbContext` (via `AddDbContext`) alongside `StoreDbContext`
- [x] 5.2 Add an `IDatabaseSeeder<AuthDbContext>` that seeds an administrative `Tenant` and `User`, assigns the `User` to the `Tenant` via `AssignToTenant`, and resolves the seeded `User.ExternalId` from the corresponding Keycloak administrative account
- [x] 5.3 Sequence `Pizzeria.Configuration`'s startup to call `MigrateDatabaseAsync<AuthDbContext>(seedData: true, ...)` before the `StoreDbContext` migration that removes `sto.Users` is applied
- [x] 5.4 Add the one-time data-migration step (raw SQL in an EF migration, or an idempotent routine in `Pizzeria.Configuration`) that copies each `sto.Users` row into `auth.Users` with no tenant membership
- [x] 5.5 Add the `Pizzeria.Store.Data` migration that drops the `Users` table from the `sto` schema, applied only after 5.1-5.4 are in place

## 6. Sample Application Cleanup

- [x] 6.1 Remove `User`, `UserByExternalIdSpecification`, `UserCreatedEvent` from `sample/Pizzeria.Store.Domain`
- [x] 6.2 Remove the `Users` `DbSet` from `sample/Pizzeria.Store.Data/StoreDbContext.cs` and remove `sample/Pizzeria.Store.Data/Configuration/UserConfiguration.cs`
- [x] 6.3 Remove `sample/Pizzeria.Store.Application/Customers/**` (`CreateCustomerCommand`, `CustomerCreatedEventHandler`, and related log message classes)
- [x] 6.4 Remove `sample/Pizzeria.Store.Api/Customers/CreateCustomerEndpoint.cs`
- [x] 6.5 Update or remove `sample/Pizzeria.Store.Contracts/Users/CreateCustomerRequest.cs` and `CustomerDto.cs` in favor of the new `Lewee.Auth.Api`/`Lewee.Auth.Contracts` DTOs, updating `Pizzeria.Common.Endpoints.StoreApi.Customers` route usage accordingly
- [x] 6.6 Update `sample/Pizzeria.Store.Web/TokenValidatedContextExtensions.cs` (and `IStoreApiClient`) to call the new create-user endpoint/route instead of the create-customer one

## 7. Test Updates and Verification

- [x] 7.1 Search the full repo for remaining references to `User`, `UserCreatedEvent`, `UserByExternalIdSpecification`, `CreateCustomerCommand`, `CreateCustomerEndpoint`, `CreateCustomerRequest`, and `CustomerDto` (`grep -r` across `src/`, `sample/`, `tests/`, `sample-tests/`) and update any missed call sites
- [x] 7.2 Update `sample-tests/Pizzeria.Store.Domain.Tests` and any other sample test project referencing the removed domain types
- [x] 7.3 Run `dotnet build --configuration Release --nologo` and confirm no errors or warnings
- [x] 7.4 Run `dotnet test --filter "FullyQualifiedName!~Integration" --configuration Release --no-build --nologo` and confirm all unit tests pass
- [ ] 7.5 Manually verify (or run, if environment allows) the `Pizzeria.Tests.Integration` Aspire-tagged suite to confirm the migrated `auth`/`sto` schemas and end-to-end first-login flow behave as expected
- [x] 7.6 Run `dotnet format` to confirm code style compliance
