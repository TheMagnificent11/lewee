namespace Lewee.StateManagement;

/// <summary>
/// Request State
/// </summary>
public abstract record class RequestState : IRequestState
{
    /// <inheritdoc/>
    public Guid CorrelationId { get; init; } = Guid.Empty;

    /// <inheritdoc/>
    public string? ErrorMessage { get; init; }
}
