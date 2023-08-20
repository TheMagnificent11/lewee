# Lewee.Domain

This package assist with implementing the domain logic of an application (architected using domain-driven design principles).

## Entity

The [Entity](./Entity.cs) class contains some common properties that developers usually like to capture.

All the properties are read-only, but have protected setters.

For example, it contains audit properties:

- `CreatedBy`
- `CreatedAtUtc`
- `ModifiedBy`
- `ModifiedAtUtc`

It also a `Timestamp` property to assist with version tracking.

The `Id` property is a `Guid`.

Finally, regarding properties, it includes an `IsDeleted` property to support soft-delete tracking.

It overrides the `Equals` method.  This returns `true` if the `Id` properties are equal or there is full referential equality.

It also overrides the `GetHashCode` method to return the string hash code after the string concatenation of the type and `Id`.

There are also some methods to allow marking and un-marking the entity as deleted, and for setting the audit fields.

## AggregateRoot, DomainEventsCollection & DomainEvent

The [AggregateRoot](./AggregateRoot.cs) class inherits of the `Entity` class and contains a [DomainEventsCollection](./DomainEventsCollection.cs).

The `DomainEventsCollection` has a `Raise` method that allows you to raise a [DomainEvent](./DomainEvent.cs).

The [Lewee.Infrastructure.Data](../Lewee.Infrastructure.Data/README.md) package contains a [DomainEventDispatcherService](../Lewee.Infrastructure.Data/DomainEventDispatcherService.cs) that dispatches the domain events stored in the database every 2500 ms; it's an [outbox pattern](https://learn.microsoft.com/en-us/azure/architecture/best-practices/transactional-outbox-cosmos).

## DomainException

The [DomainException](./DomainException.cs`) class can be used to raise errors when domain rules are violated.

The [Lewee.Application](../Lewee.Application/README.md) package contains has a [DomainExceptionBehavior](../Lewee.Application/Mediation/Behaviors/DomainExceptionBehavior.cs) that catches these exceptions, logs them as an informational log entry and returns a failure result.
