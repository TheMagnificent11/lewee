namespace Lewee.StateManagement;

/// <summary>
/// Command State
/// </summary>
public abstract record class CommandState : IRequestState
{
    /// <inheritdoc />
    public bool IsLoading { get; init; }

    /// <inheritdoc />
    public bool IsSaving { get; init; }

    /// <inheritdoc />
    public Guid CorrelationId { get; init; }

    /// <inheritdoc />
    public string? ErrorMessage { get; init; }
}
