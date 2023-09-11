namespace Lewee.Blazor.Fluxor;

/// <summary>
/// Request State Interface
/// </summary>
public interface IRequestState
{
    /// <summary>
    /// Gets the request correlation ID
    /// </summary>
    Guid CorrelationId { get; init; }

    /// <summary>
    /// Gets the request error message
    /// </summary>
    string? ErrorMessage { get; init; }
}
