using MediatR;

namespace Lewee.Domain;

/// <summary>
/// Domain Event
/// </summary>
public abstract class DomainEvent : INotification
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEvent"/> class
    /// </summary>
    protected DomainEvent()
    {
        this.CorrelationId = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the correlation ID of the event
    /// </summary>
    public Guid CorrelationId { get; protected set; }

    /// <summary>
    /// Gets or sets the user ID
    /// </summary>
    public string? UserId { get; set; }
}
