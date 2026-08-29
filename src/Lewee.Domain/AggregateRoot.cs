namespace Lewee.Domain;

/// <summary>
/// Aggregate Root
/// </summary>
public abstract class AggregateRoot : Entity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot"/> class
    /// </summary>
    protected AggregateRoot()
        : base(Guid.NewGuid())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot"/> class
    /// </summary>
    /// <param name="id">Entity ID</param>
    protected AggregateRoot(Guid id)
        : base(id)
    {
    }

    /// <summary>
    /// Gets the domain events collection
    /// </summary>
    public DomainEventsCollection DomainEvents { get; } = new DomainEventsCollection();
}
