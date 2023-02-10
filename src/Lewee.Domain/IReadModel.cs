namespace Lewee.Domain;

/// <summary>
/// Read Model Interface
/// </summary>
public interface IReadModel
{
    /// <summary>
    /// Gets the correlation ID of the event
    /// </summary>
    Guid CorrelationId { get; }
}
