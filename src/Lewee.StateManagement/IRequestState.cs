namespace Lewee.StateManagement;

/// <summary>
/// Request State Interface
/// </summary>
/// <typeparam name="T">Data type</typeparam>
public interface IRequestState<T>
    where T : class
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
    /// Gets the data
    /// </summary>
    T Data { get; init; }

    /// <summary>
    /// Gets the request error message
    /// </summary>
    string? ErrorMessage { get; init; }
}
