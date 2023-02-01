using Lewee.Fluxor.Actions;

namespace Lewee.Fluxor;

/// <summary>
/// Reducer Extension Methods
/// </summary>
public static class ReducerExtensions
{
    /// <summary>
    /// On query reducer
    /// </summary>
    /// <typeparam name="TState">Query state type</typeparam>
    /// <typeparam name="TStateData">Query state data type</typeparam>
    /// <typeparam name="TAction">Query action type</typeparam>
    /// <param name="state">Query state</param>
    /// <param name="action">Query action</param>
    /// <returns>Updated state</returns>
    public static TState OnQuery<TState, TStateData, TAction>(this TState state, TAction action)
        where TState : BaseQueryState<TStateData>
        where TStateData : class
        where TAction : IRequestAction
    {
        return state with { CorrelationId = action.CorrelationId, Data = default, ErrorMessage = null };
    }

    /// <summary>
    /// On query success reducer
    /// </summary>
    /// <typeparam name="TState">Query state type</typeparam>
    /// <typeparam name="TStateData">Query state data type</typeparam>
    /// <typeparam name="TAction">Query action type</typeparam>
    /// <param name="state">Query state</param>
    /// <param name="action">Query action</param>
    /// <returns>Updated state</returns>
    public static TState OnQuerySuccess<TState, TStateData, TAction>(this TState state, TAction action)
        where TState : BaseQueryState<TStateData>
        where TStateData : class
        where TAction : IQuerySuccessAction<TStateData>
    {
        return state with { Data = action.Data };
    }

    /// <summary>
    /// On query success reducer
    /// </summary>
    /// <typeparam name="TState">Query state type</typeparam>
    /// <typeparam name="TStateData">Query state data type</typeparam>
    /// <typeparam name="TAction">Query action type</typeparam>
    /// <param name="state">Query state</param>
    /// <param name="action">Query action</param>
    /// <returns>Updated state</returns>
    public static TState OnQueryError<TState, TStateData, TAction>(this TState state, TAction action)
        where TState : BaseQueryState<TStateData>
        where TStateData : class
        where TAction : IRequestErrorAction
    {
        return state with { ErrorMessage = action.ErrorMessage };
    }
}
