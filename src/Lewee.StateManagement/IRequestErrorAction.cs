namespace Lewee.StateManagement;

/// <summary>
/// Request Error Action Interface
/// </summary>
public interface IRequestErrorAction
{
    /// <summary>
    /// Gets the correlation ID
    /// </summary>
    Guid CorrelationId { get; init; }

    /// <summary>
    /// Gets the error message
    /// </summary>
    string ErrorMessage { get; init; }
}
