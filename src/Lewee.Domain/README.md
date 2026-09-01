# Lewee.Domain

Domain layer abstractions for implementing applications using Domain-Driven Design (DDD) principles.

## Purpose

This package provides the foundational building blocks for creating domain models following DDD patterns, including entities, aggregate roots, value objects, domain events, specifications, and repository interfaces.

## Dependencies

- `MediatR` - For domain event notification support

## Components

### Base Classes

#### AuditableRecord

Base class for records that track audit information:

| Property | Type | Description |
| ---------- | ------ | ------------- |
| `Id` | `Guid` | Unique identifier |
| `CreatedBy` | `string` | User who created the record |
| `CreatedAtUtc` | `DateTime` | UTC timestamp of creation |
| `ModifiedBy` | `string` | User who last modified the record |
| `ModifiedAtUtc` | `DateTime` | UTC timestamp of last modification |

#### Entity

Extends `AuditableRecord` with soft-delete support via the `ISoftDeleteEntity` interface:

- `IsDeleted` property for soft-delete tracking
- `Delete()` and `Undelete()` methods
- Equality based on `Id` property
- Hash code based on type and `Id`

#### AggregateRoot

Extends `Entity` with domain event support:

```csharp
public class Order : AggregateRoot
{
    public void Complete()
    {
        this.Status = OrderStatus.Completed;
        this.DomainEvents.Raise(new OrderCompletedEvent(correlationId, this.Id));
    }
}
```

The `DomainEventsCollection` stores raised events until they are dispatched by `Lewee.Infrastructure.Data`.

#### ValueObject\<T>

Base class for immutable value objects with equality based on component values:

```csharp
public class Address : ValueObject<Address>
{
    public string Street { get; }
    public string City { get; }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
    }
}
```

#### Relationship

Base class for many-to-many relationship entities, extending `AuditableRecord`.

#### EnumEntity\<TKey>

Represents an enum value as a database entity with `Id` and `Name` properties, useful for lookup tables.

### Domain Events

#### DomainEvent

Base class for domain events implementing MediatR's `INotification`:

| Property | Type | Description |
|----------|------|-------------|
| `CorrelationId` | `Guid` | Request correlation ID for tracing |
| `UserId` | `string?` | Optional user ID who triggered the event |
| `EventDateTime` | `DateTime` | When the event occurred |

#### DomainEventsCollection

Collection for raising and storing domain events on aggregate roots:

```csharp
this.DomainEvents.Raise(new OrderCreatedEvent(correlationId));
```

#### DomainEventReference

Serialized reference to a domain event stored in the database for the outbox pattern.

### Interfaces

#### IRepository\<T>

Repository interface for aggregate root data access:

```csharp
public interface IRepository<T> where T : AggregateRoot
{
    Task<T?> RetrieveByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<T>> AllAsync(CancellationToken cancellationToken = default);
    Task<List<T>> QueryAsync(QuerySpecification<T> spec, CancellationToken cancellationToken = default);
    Task<T?> QueryOneAsync(QuerySpecification<T> spec, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

#### ISpecification\<T>

Generic specification pattern interface for business rule validation:

```csharp
public class OrderCanBeShippedSpecification : ISpecification<Order>
{
    public bool IsValid(Order order) => order.Status == OrderStatus.Paid;
}
```

#### IQueryProjection

Marker interface for query projections with a correlation ID.

#### IQueryProjectionService

Service interface for executing query projections.

#### ISoftDeleteEntity

Marker interface for entities supporting soft delete.

#### IServiceBusEvent

Marker interface for events that should be published to a service bus.

### Query Specifications

#### QuerySpecification\<T>

Fluent builder for repository queries with filtering and eager loading:

```csharp
public class ActiveOrdersByCustomerSpec : QuerySpecification<Order>
{
    public ActiveOrdersByCustomerSpec(Guid customerId)
    {
        Query
            .Where(o => o.CustomerId == customerId)
            .Where(o => !o.IsDeleted)
            .Include(o => o.Items)
            .ThenInclude<OrderItem, Product>(i => i.Product);
    }
}
```

### Exceptions

#### DomainException

Exception for domain rule violations, caught by `DomainExceptionBehavior` in `Lewee.Application`:

```csharp
throw new DomainException("Order cannot be modified after completion");
```

## Integration with Other Lewee Packages

| Package | Integration |
| --------- | ------------- |
| `Lewee.Application` | `DomainExceptionBehavior` handles `DomainException` in MediatR pipeline |
| `Lewee.Infrastructure.Data` | Implements `IRepository<T>`, dispatches domain events via outbox pattern |
| `Lewee.Infrastructure.Auth` | Implements `IAuthenticatedUserService` from `Lewee.Common` |

## Architecture Benefits

- **Clean Domain Model**: Entities focus on business logic, not infrastructure
- **Event-Driven**: Domain events enable decoupled reactions to state changes
- **Outbox Pattern**: Domain events are persisted for reliable async dispatch
- **Specification Pattern**: Encapsulate and reuse query logic
- **Soft Delete**: Track deleted records without losing data
- **Audit Trail**: Automatic tracking of who created/modified records
