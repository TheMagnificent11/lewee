namespace Lewee.StateManagement;

/// <summary>
/// Query State
/// </summary>
/// <typeparam name="T">Query data type</typeparam>
public abstract record QueryState<T> : RequestState
    where T : class
{
    /// <summary>
    /// Gets the queried data
    /// </summary>
    public T? Data { get; init; }
}
