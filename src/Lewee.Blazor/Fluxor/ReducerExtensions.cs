using Lewee.Blazor.Fluxor.Actions;

namespace Lewee.Blazor.Fluxor;

/// <summary>
/// Reducer Extension Methods
/// </summary>
public static class ReducerExtensions
{
    /// <summary>
    /// On request reducer
    /// </summary>
    /// <typeparam name="TState">Request state type</typeparam>
    /// <typeparam name="TAction">Request action type</typeparam>
    /// <param name="state">Request state</param>
    /// <param name="action">Request action</param>
    /// <returns>Updated state</returns>
    public static TState OnRequest<TState, TAction>(this TState state, TAction action)
        where TState : RequestState
        where TAction : IRequestAction
    {
        return state with { CorrelationId = action.CorrelationId, ErrorMessage = null };
    }

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
        where TState : QueryState<TStateData>
        where TStateData : class
        where TAction : IRequestAction
    {
        return state.OnRequest(action) with { Data = default };
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
        where TState : QueryState<TStateData>
        where TStateData : class
        where TAction : IQuerySuccessAction<TStateData>
    {
        return state with { Data = action.Data };
    }

    /// <summary>
    /// On request error reducer
    /// </summary>
    /// <typeparam name="TState">Request state type</typeparam>
    /// <typeparam name="TAction">Request action type</typeparam>
    /// <param name="state">Query state</param>
    /// <param name="action">Query action</param>
    /// <returns>Updated state</returns>
    public static TState OnRequestError<TState, TAction>(this TState state, TAction action)
        where TState : RequestState
        where TAction : IRequestErrorAction
    {
        return state with { ErrorMessage = action.ErrorMessage };
    }
}
