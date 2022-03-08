namespace Saji.Domain;

/// <summary>
/// Domain Event Interface
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets or sets the correlation ID
    /// </summary>
    Guid CorrelationId { get; set; }
}
