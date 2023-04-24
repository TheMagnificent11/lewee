# Lewee.Domain

This package assist with implementing the domain logic of an application.

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
