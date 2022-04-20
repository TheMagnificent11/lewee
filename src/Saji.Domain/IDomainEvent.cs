namespace Saji.Domain;

/// <summary>
/// Domain Event Interface
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the correlation ID of the event
    /// </summary>
    Guid CorrelationId { get; }
}
