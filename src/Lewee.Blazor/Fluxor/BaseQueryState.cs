namespace Lewee.Blazor.Fluxor;

/// <summary>
/// Query State
/// </summary>
/// <typeparam name="T">Query data type</typeparam>
public abstract record BaseQueryState<T> : BaseRequestState
    where T : class
{
    /// <summary>
    /// Gets the queried data
    /// </summary>
    public T? Data { get; init; }
}
