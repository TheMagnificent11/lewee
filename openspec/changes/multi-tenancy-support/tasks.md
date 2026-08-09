## 1. Lewee.Auth.Domain (new project)

- [ ] 1.1 Create `src/Lewee.Auth.Domain` project referencing `Lewee.Domain` only, and add it to `lewee.slnx`
- [ ] 1.2 Add `Tenant : AggregateRoot` with a `Name` (or equivalent) property, a `Users` collection navigation, and a `CreateUser(externalId, correlationId)` factory method that constructs a `User`, adds it to `Users`, and raises `UserCreatedEvent`
- [ ] 1.3 Move `User` from `sample/Pizzeria.Store.Domain/User.cs` into `Lewee.Auth.Domain`, changing its base type from `AggregateRoot` to `Entity`, adding a required `TenantId` property, and restricting construction to be invoked only via `Tenant.CreateUser(...)`
- [ ] 1.4 Move `UserCreatedEvent` from `sample/Pizzeria.Store.Domain/UserCreatedEvent.cs` into `Lewee.Auth.Domain`, adding a `TenantId` property
- [ ] 1.5 Move `UserByExternalIdSpecification` from `sample/Pizzeria.Store.Domain/UserByExternalIdSpecification.cs` into `Lewee.Auth.Domain`, updating it to filter by both `TenantId` and `ExternalId` per the `auth/tenant-management` spec
- [ ] 1.6 Add a `TenantCreatedEvent` (or reuse an existing pattern) raised when a `Tenant` is created
- [ ] 1.7 Create `tests/Lewee.Auth.Domain.Tests.Unit` covering: tenant creation, user creation via `Tenant.CreateUser`, duplicate external ID within a tenant, and same external ID across different tenants (per `auth/tenant-management` spec scenarios)

## 2. Lewee.Auth.Infrastructure.Data (new project)

- [ ] 2.1 Create `src/Lewee.Auth.Infrastructure.Data` project referencing `Lewee.Infrastructure.Data` and `Lewee.Auth.Domain`, and add it to `lewee.slnx`
- [ ] 2.2 Add `AuthDbContext : ApplicationDbContext<AuthDbContext>` with `Schema => "auth"` and `DbSet<Tenant> Tenants`
- [ ] 2.3 Add `TenantConfiguration : AggregateRootConfiguration<Tenant>` configuring the owned `Users` collection and the `(TenantId, ExternalId)` unique index on `User`
- [ ] 2.4 Add `UserConfiguration : EntityConfiguration<User>` (not `AggregateRootConfiguration`, since `User` is no longer an aggregate root) for any `User`-specific column configuration not covered by `TenantConfiguration`
- [ ] 2.5 Add the initial EF Core migration creating the `auth` schema with `Tenants`/`Users` tables
- [ ] 2.6 Create `tests/Lewee.Auth.Infrastructure.Data.Tests.Unit` covering the EF configuration (schema name, unique index scoped per tenant), following existing patterns in `tests/Lewee.Infrastructure.Data.Tests.Unit`

## 3. Lewee.Auth.Application (new project)

- [ ] 3.1 Create `src/Lewee.Auth.Application` project referencing `Lewee.Application` and `Lewee.Auth.Domain`, and add it to `lewee.slnx`
- [ ] 3.2 Add `CreateUserCommand(string ExternalUserId, Guid TenantId) : ICommand` with a `Validator` (non-empty `ExternalUserId` within `User.FieldLengths.ExternalId`, non-empty `TenantId`) ported from `sample/Pizzeria.Store.Application/Customers/CreateCustomerCommand.cs`
- [ ] 3.3 Add the `CreateUserCommand.Handler`, loading the `Tenant` via `IRepository<Tenant>`, using the tenant-scoped `UserByExternalIdSpecification` for idempotency, and calling `tenant.CreateUser(...)` before saving
- [ ] 3.4 Port `CustomerCreatedEventHandler`/`CustomerCreatedEventHandlerLogMessages`/`CreateCustomerCommandLogMessages` to `Lewee.Auth.Application`, renamed to match `UserCreatedEvent`/`CreateUserCommand`
- [ ] 3.5 Create `tests/Lewee.Auth.Application.Tests.Unit`, porting the existing `CreateCustomerCommand` test coverage to `CreateUserCommand` (valid create, idempotent duplicate within tenant, same external ID across tenants)

## 4. Lewee.Auth.Api (new project)

- [ ] 4.1 Create `src/Lewee.Auth.Api` project referencing `Lewee.Infrastructure.FastEndpoints` and `Lewee.Auth.Application`, and add it to `lewee.slnx`
- [ ] 4.2 Add `CreateUserRequest`/`UserDto` contracts (in `Lewee.Auth.Api` or a new `Lewee.Auth.Contracts` project, matching how `Pizzeria.Store.Contracts.Users` is structured today) with `ExternalUserId` and `TenantId` (or a resolved-tenant mechanism per design Open Questions)
- [ ] 4.3 Add `CreateUserEndpoint : CommandEndpoint<CreateUserRequest>` (anonymous access allowed) ported from `sample/Pizzeria.Store.Api/Customers/CreateCustomerEndpoint.cs`
- [ ] 4.4 Add a project reference from `Pizzeria.Store.Api` to `Lewee.Auth.Api` (and `Lewee.Auth.Application`/`Lewee.Auth.Infrastructure.Data` for DI registration) so the endpoint is exposed by the sample API
- [ ] 4.5 Register `AddLeweeDatabaseServices<AuthDbContext>(...)` and any `Lewee.Auth.Application`/`Lewee.Auth.Api` DI extension methods in `Pizzeria.Store.Api`'s `Program.cs`

## 5. Database Migration and Seeding

- [ ] 5.1 Update `Pizzeria.Configuration` to register `AuthDbContext` (via `AddDbContext`) alongside `StoreDbContext`
- [ ] 5.2 Add an `IDatabaseSeeder<AuthDbContext>` that seeds a default/administrative `Tenant` and `User`, resolving the seeded `User.ExternalId` from the corresponding Keycloak administrative account
- [ ] 5.3 Sequence `Pizzeria.Configuration`'s startup to call `MigrateDatabaseAsync<AuthDbContext>(seedData: true, ...)` before the `StoreDbContext` migration that removes `sto.Users` is applied
- [ ] 5.4 Add the one-time data-migration step (raw SQL in an EF migration, or an idempotent routine in `Pizzeria.Configuration`) that creates a default `Tenant` for pre-existing data and copies each `sto.Users` row into `auth.Users` under that `Tenant`
- [ ] 5.5 Add the `Pizzeria.Store.Data` migration that drops the `Users` table from the `sto` schema, applied only after 5.1-5.4 are in place

## 6. Sample Application Cleanup

- [ ] 6.1 Remove `User`, `UserByExternalIdSpecification`, `UserCreatedEvent` from `sample/Pizzeria.Store.Domain`
- [ ] 6.2 Remove the `Users` `DbSet` from `sample/Pizzeria.Store.Data/StoreDbContext.cs` and remove `sample/Pizzeria.Store.Data/Configuration/UserConfiguration.cs`
- [ ] 6.3 Remove `sample/Pizzeria.Store.Application/Customers/**` (`CreateCustomerCommand`, `CustomerCreatedEventHandler`, and related log message classes)
- [ ] 6.4 Remove `sample/Pizzeria.Store.Api/Customers/CreateCustomerEndpoint.cs`
- [ ] 6.5 Update or remove `sample/Pizzeria.Store.Contracts/Users/CreateCustomerRequest.cs` and `CustomerDto.cs` in favor of the new `Lewee.Auth.Api`/`Lewee.Auth.Contracts` DTOs, updating `Pizzeria.Common.Endpoints.StoreApi.Customers` route usage accordingly
- [ ] 6.6 Update `sample/Pizzeria.Store.Web/TokenValidatedContextExtensions.cs` (and `IStoreApiClient`) to call the new create-user endpoint/route instead of the create-customer one

## 7. Test Updates and Verification

- [ ] 7.1 Search the full repo for remaining references to `User`, `UserCreatedEvent`, `UserByExternalIdSpecification`, `CreateCustomerCommand`, `CreateCustomerEndpoint`, `CreateCustomerRequest`, and `CustomerDto` (`grep -r` across `src/`, `sample/`, `tests/`, `sample-tests/`) and update any missed call sites
- [ ] 7.2 Update `sample-tests/Pizzeria.Store.Domain.Tests` and any other sample test project referencing the removed domain types
- [ ] 7.3 Run `dotnet build --configuration Release --nologo` and confirm no errors or warnings
- [ ] 7.4 Run `dotnet test --filter "FullyQualifiedName!~Integration" --configuration Release --no-build --nologo` and confirm all unit tests pass
- [ ] 7.5 Manually verify (or run, if environment allows) the `Pizzeria.Tests.Integration` Aspire-tagged suite to confirm the migrated `auth`/`sto` schemas and end-to-end first-login flow behave as expected
- [ ] 7.6 Run `dotnet format` to confirm code style compliance
