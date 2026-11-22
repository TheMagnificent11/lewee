# Lewee.Infrastructure.Data

Entity Framework Core integration for domain-driven design applications with repository pattern, domain event dispatching, and audit tracking.

## Purpose

This package provides the data access layer infrastructure for applications using `Lewee` dependencies. It includes Entity Framework Core configuration helpers, repository implementation, domain event dispatching via the outbox pattern, and automatic audit field population.

## Dependencies

- `Lewee.Domain` - Domain layer abstractions
- `MediatR` - For domain event notification dispatching
- `Microsoft.EntityFrameworkCore` - ORM framework
- `Microsoft.EntityFrameworkCore.Relational` - Relational database support
- `Microsoft.Extensions.Configuration.Binder` - Configuration binding
- `Microsoft.Extensions.DependencyModel` - Assembly scanning
- `Microsoft.Extensions.Hosting.Abstractions` - Hosting abstractions

## Components

### Entity Configuration Classes

Abstract base classes for configuring Entity Framework entity mappings:

| Class | Purpose |
| ------- | --------- |
| `AuditableRecordConfiguration<T>` | Base configuration for entities extending `AuditableRecord` |
| `EntityConfiguration<T>` | Configuration for entities with soft-delete support |
| `AggregateRootConfiguration<T>` | Configuration for aggregate roots with domain events |
| `RelationshipConfiguration<T>` | Configuration for many-to-many relationship entities |
| `EnumEntityConfiguration<TEnum>` | Configuration for enum lookup tables |
| `DomainEventReferenceConfiguration` | Configuration for domain event outbox table |
| `QueryProjectionReferenceConfiguration` | Configuration for query projection tracking |

### Database Context

#### ApplicationDbContext\<TContext>

Abstract base class for application database contexts with domain event and query projection support:

```cs
public class MyDbContext : ApplicationDbContext<MyDbContext>
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
}
```

### Interceptors

| Interceptor | Purpose |
| ------------- | --------- |
| `AuditDetailsSaveChangesInterceptor` | Populates `CreatedBy`, `CreatedAtUtc`, `ModifiedBy`, `ModifiedAtUtc` fields |
| `DomainEventSaveChangesInterceptor<TContext>` | Stores domain events to outbox table on save |
| `DomainEventsTransactionInterceptor` | Dispatches domain events after transaction commit |

### Repository

#### Repository\<TAggregateRoot>

Generic repository implementation for aggregate roots:

```cs
public interface IOrderRepository : IRepository<Order> { }

// Automatically registered via AddLeweeDatabaseServices
```

### Domain Event Dispatching

#### DomainEventDispatcher

Dispatches domain events from the outbox table via MediatR after successful database transactions.

### Query Projections

#### QueryProjectionService

Service for executing query projections with tracking support.

### Database Seeding

#### IDatabaseSeeder\<TDbContext>

Interface for implementing database seeders:

```cs
public class MyDbSeeder : IDatabaseSeeder<MyDbContext>
{
    public async Task SeedAsync(MyDbContext context, CancellationToken cancellationToken)
    {
        // Seed data
    }
}
```

## Configuration

### Basic Setup

```cs
// Without DB Seeder
services
    .AddDbContextFactory<MyDbContext>(options => options.UseSqlServer(connectionString))
    .AddLeweeDatabaseServices<MyDbContext>(typeof(MyDomainEntity).Assembly);
```

### With Database Seeder

```cs
services
    .AddDbContextFactory<MyDbContext>(options => options.UseSqlServer(connectionString))
    .AddLeweeDatabaseServicesWithSeeder<MyDbContext, MyDbSeeder>(typeof(MyDomainEntity).Assembly);
```

### With Interceptors (recommended)

```cs
services
    .AddDbContextFactory<MyDbContext>((provider, options) =>
    {
        options.UseSqlServer(connectionString);
        options.AddAuditInterceptor(provider);
        options.AddDomainEventInterceptors<MyDbContext>(provider);
    })
    .AddLeweeDatabaseServices<MyDbContext>(typeof(MyDomainEntity).Assembly);
```

## Extension Methods

### ApplicationDbContextOptionsBuilderExtensions

| Method | Purpose |
|--------|---------|
| `AddAuditInterceptor` | Adds audit details save changes interceptor |
| `AddDomainEventInterceptors<TContext>` | Adds domain event save changes and transaction interceptors |

### EntityTypeBuilderExtensions

| Method | Purpose |
|--------|---------|
| `ConfigureSoftDelete` | Adds global query filter for soft-deleted entities |

### EntityEntryExtensions

| Method | Purpose |
|--------|---------|
| `HasChangedOwnedEntities` | Checks if owned entities have changed |

## Sample Usage

See the [Pizzeria Store Data project](../../sample/Pizzeria.Store.Data/) for a complete implementation example.
