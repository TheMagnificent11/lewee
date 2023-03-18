namespace Lewee.Domain;

/// <summary>
/// Base Aggregate Root
/// </summary>
public abstract class BaseAggregateRoot : BaseEntity, IAggregateRoot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseAggregateRoot"/> class
    /// </summary>
    protected BaseAggregateRoot()
        : base(Guid.NewGuid())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseAggregateRoot"/> class
    /// </summary>
    /// <param name="id">Entity ID</param>
    protected BaseAggregateRoot(Guid id)
        : base(id)
    {
    }

    /// <summary>
    /// Gets the domain events collection
    /// </summary>
    public DomainEventsCollection DomainEvents { get; } = new DomainEventsCollection();
}
