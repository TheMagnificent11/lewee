namespace Lewee.Domain;

/// <summary>
/// Domain Event
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEvent"/> class
    /// </summary>
    protected DomainEvent()
    {
        this.CorrelationId = Guid.NewGuid();
    }

    /// <inheritdoc />
    public Guid CorrelationId { get; protected set; }

    /// <inheritdoc />
    public string? UserId { get; set; }
}
