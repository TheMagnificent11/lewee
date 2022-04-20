namespace Saji.Domain;

/// <summary>
/// Base Domain Event
/// </summary>
public abstract class BaseDomainEvent : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseDomainEvent"/> class
    /// </summary>
    protected BaseDomainEvent()
    {
        this.CorrelationId = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the correlation ID of the event
    /// </summary>
    public Guid CorrelationId { get; protected set; }
}
