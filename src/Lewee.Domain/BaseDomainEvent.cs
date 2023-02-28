namespace Lewee.Domain;

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

    /// <inheritdoc />
    public Guid CorrelationId { get; protected set; }

    /// <inheritdoc />
    public string? ClientId { get; set; }
}
