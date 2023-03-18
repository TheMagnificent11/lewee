namespace Lewee.Domain;

/// <summary>
/// Query Projection Interface
/// </summary>
public interface IQueryProjection
{
    /// <summary>
    /// Gets the correlation ID of the event
    /// </summary>
    Guid CorrelationId { get; }
}
