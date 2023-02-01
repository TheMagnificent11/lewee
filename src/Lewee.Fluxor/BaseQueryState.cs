namespace Lewee.Fluxor;

/// <summary>
/// Query State
/// </summary>
/// <typeparam name="T">Query data type</typeparam>
public abstract record BaseQueryState<T> : IRequestState
    where T : class
{
    /// <inheritdoc/>
    public abstract string RequestType { get; }

    /// <inheritdoc/>
    public Guid CorrelationId { get; init; } = Guid.Empty;

    /// <summary>
    /// Gets the queried data
    /// </summary>
    public T? Data { get; init; }

    /// <inheritdoc/>
    public string? ErrorMessage { get; init; }
}
