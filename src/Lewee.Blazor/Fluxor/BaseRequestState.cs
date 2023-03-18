namespace Lewee.Blazor.Fluxor;

/// <summary>
/// Base Request State
/// </summary>
public abstract record class BaseRequestState : IRequestState
{
    /// <inheritdoc/>
    public abstract string RequestType { get; }

    /// <inheritdoc/>
    public Guid CorrelationId { get; init; } = Guid.Empty;

    /// <inheritdoc/>
    public string? ErrorMessage { get; init; }
}
