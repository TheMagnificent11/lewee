namespace Lewee.StateManagement;

/// <summary>
/// Request State Interface
/// </summary>
public interface IRequestState
{
    /// <summary>
    /// Gets a value indicating whether the state is loading
    /// </summary>
    bool IsLoading { get; init; }

    /// <summary>
    /// Gets a value indicating whether the state is saving
    /// </summary>
    bool IsSaving { get; init; }

    /// <summary>
    /// Gets the request correlation ID
    /// </summary>
    Guid CorrelationId { get; init; }

    /// <summary>
    /// Gets the request error message
    /// </summary>
    string? ErrorMessage { get; init; }
}
