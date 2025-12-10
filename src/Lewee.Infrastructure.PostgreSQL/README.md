# Lewee.Infrastructure.PostgreSQL

PostgreSQL-specific database configuration for Entity Framework Core with Npgsql.

## Purpose

This package provides PostgreSQL-specific configuration for Entity Framework Core, extending the base database functionality from `Lewee.Infrastructure.Data`. It handles Npgsql setup, schema support, migration history table configuration, and exception processing.

## Dependencies

- `Lewee.Infrastructure.Data` - Base Entity Framework configuration
- `Npgsql.EntityFrameworkCore.PostgreSQL` - PostgreSQL provider for EF Core
- `EntityFrameworkCore.Exceptions.PostgreSQL` - Exception processor for PostgreSQL

## Components

### PostgreSqlConfiguration

Static configuration class with the `AddLeweePostgreSQL` extension method for registering PostgreSQL database contexts.

**Key Features:**

- Configures Npgsql as the database provider
- Supports optional schema specification for multi-tenant scenarios
- Configures migrations history table within the specified schema
- Integrates `EntityFrameworkCore.Exceptions.PostgreSQL` for better exception handling
- Automatically adds audit and domain event interceptors from `Lewee.Infrastructure.Data`

## Configuration

```cs
using Lewee.Infrastructure.PostgreSQL;

// Basic configuration (default schema)
services.AddLeweePostgreSQL<MyDbContext>(
    connectionString: "Host=localhost;Database=mydb;...",
    domainAssembly: typeof(MyDomainEntity).Assembly);

// Configuration with schema (for multi-tenant scenarios)
services.AddLeweePostgreSQL<MyDbContext>(
    connectionString: "Host=localhost;Database=mydb;...",
    domainAssembly: typeof(MyDomainEntity).Assembly,
    schema: "tenant1");
```

## Features

### Schema Support

When a schema is specified:

- The migrations history table (`__EFMigrationsHistory`) is placed within the specified schema
- Useful for multi-tenant applications with schema-per-tenant isolation

### Exception Processing

Uses `EntityFrameworkCore.Exceptions.PostgreSQL` to convert PostgreSQL-specific exceptions into more meaningful .NET exceptions, improving error handling for:

- Unique constraint violations
- Foreign key violations
- Not null constraint violations

### Interceptor Integration

Automatically configures the following interceptors from `Lewee.Infrastructure.Data`:

- `AuditDetailsSaveChangesInterceptor` - Populates created/modified audit fields
- `DomainEventSaveChangesInterceptor` - Dispatches domain events after save

## Sample Usage

See the [Pizzeria Store Data project](../../sample/Pizzeria.Store.Data/) for a complete implementation example with PostgreSQL.
