namespace Lewee.Blazor.Fluxor.Actions;

/// <summary>
/// Query Success Action Interface
/// </summary>
/// <typeparam name="T">Query data type</typeparam>
public interface IQuerySuccessAction<out T> : IRequestSuccessAction
    where T : class
{
    /// <summary>
    /// Gets the queried data
    /// </summary>
    T Data { get; }
}
