namespace Lewee.StateManagement;

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
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    public T? Data { get; init; } // TODO: fix nullability
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

    /// <inheritdoc />
    public string? ErrorMessage { get; init; }
}
