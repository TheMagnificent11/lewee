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
    /// <param name="correlationId">Correlation ID</param>
    protected DomainEvent(Guid correlationId)
    {
        this.CorrelationId = correlationId;
    }

    /// <summary>
    /// Gets or sets the correlation ID of the event
    /// </summary>
    public Guid CorrelationId { get; protected set; }

    /// <summary>
    /// Gets or sets the user ID
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets the date-time of the event
    /// </summary>
    public DateTime EventDateTime { get; init; }
}
