## Purpose

Defines the persistence boundary for authorization data: a dedicated `auth` database schema and `AuthDbContext`, and the migration path for moving existing `User` data out of the `sto` (Pizzeria Store) schema.

## ADDED Requirements

### Requirement: Authorization data is persisted in a dedicated auth schema
`Tenant` and `User` data SHALL be persisted via a database context whose default schema is `auth`, separate from any application-specific schema (such as `sto`). This context SHALL NOT contain application-specific aggregates (e.g. `Order`, `Pizza`). `Tenant` and `User` SHALL each be persisted as their own top-level aggregate (each with its own table), and the membership between them SHALL be persisted in a separate table.

#### Scenario: Auth tables are created in the auth schema
- **WHEN** the database context responsible for `Tenant`/`User` persistence applies its migrations
- **THEN** the resulting `Tenants`, `Users`, and tenant-membership tables SHALL exist in the `auth` schema

### Requirement: Existing user data is migrated from the sto schema to the auth schema
Existing rows in the `sto.Users` table SHALL be migrated into the new `auth.Users` table as part of the rollout of this change, with no tenant membership, and the `sto.Users` table SHALL be removed once migration completes.

#### Scenario: Pre-existing users are preserved after migration
- **WHEN** the database migration that introduces the `auth` schema is applied to a database containing existing rows in `sto.Users`
- **THEN** each existing user SHALL have a corresponding row in `auth.Users`, with no tenant membership, and no data SHALL be lost

#### Scenario: sto schema no longer exposes user data
- **WHEN** the migration described above has been applied
- **THEN** the `sto.Users` table SHALL no longer exist, and `StoreDbContext` SHALL NOT expose a `Users` `DbSet`

### Requirement: Database migration and seeding covers the auth database context
The application's startup/configuration migration process SHALL migrate and, where configured, seed the auth database context in addition to any application-specific database contexts, including creating an initial administrative tenant and user (assigned to that tenant) whose external ID corresponds to a Keycloak-provisioned administrative account.

#### Scenario: First-run environment provisioning
- **WHEN** the configuration/migration process runs against a fresh environment
- **THEN** the `auth` schema SHALL be migrated, an administrative `Tenant` and `User` SHALL be seeded and assigned to each other, and the seeded `User`'s external ID SHALL match the corresponding Keycloak user's identifier
