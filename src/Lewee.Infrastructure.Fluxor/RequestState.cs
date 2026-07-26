namespace Lewee.Infrastructure.Fluxor;

/// <summary>
/// Command State
/// </summary>
/// <typeparam name="T">Data type</typeparam>
public abstract record class RequestState<T> : IRequestState<T>
    where T : class
{
    /// <inheritdoc />
    public bool IsLoading { get; init; }

    /// <inheritdoc />
    public bool IsSaving { get; init; }

    /// <inheritdoc />
    public Guid CorrelationId { get; init; }

    /// <inheritdoc />
    public T? Data { get; init; }

    /// <inheritdoc />
    public string? ErrorMessage { get; init; }
}
