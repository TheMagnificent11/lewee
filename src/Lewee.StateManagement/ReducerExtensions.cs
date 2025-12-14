namespace Lewee.StateManagement;

/// <summary>
/// Reducer Extension Methods
/// </summary>
public static class ReducerExtensions
{
    /// <summary>
    /// On command reducer
    /// </summary>
    /// <typeparam name="TState">Request state type</typeparam>
    /// <typeparam name="TAction">Request action type</typeparam>
    /// <param name="state">Requet state</param>
    /// <param name="action">Request action</param>
    /// <returns>Updated state</returns>
    public static TState OnCommand<TState, TAction>(this TState state, TAction action)
        where TState : CommandState
        where TAction : IRequestAction
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);

        return state with
        {
            IsSaving = true,
            CorrelationId = action.CorrelationId,
            ErrorMessage = null,
        };
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
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);

        return state with
        {
            IsLoading = true,
            CorrelationId = action.CorrelationId,
            Data = null,
            ErrorMessage = null,
        };
    }

    /// <summary>
    /// On command success
    /// </summary>
    /// <typeparam name="TState">Request state type</typeparam>
    /// <typeparam name="TAction">Request action type</typeparam>
    /// <param name="state">Requet state</param>
    /// <param name="action">Request action</param>
    /// <returns>Updated state</returns>
    public static TState OnCommandSuccess<TState, TAction>(this TState state, TAction action)
        where TState : CommandState
        where TAction : IRequestSuccessAction
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);

        return state with
        {
            IsSaving = false,
        };
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
    public static TState OnQuerySuccess<TState, TStateData, TAction>(
        this TState state,
        TAction action)
        where TState : QueryState<TStateData>
        where TStateData : class
        where TAction : IQuerySuccessAction<TStateData>
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);

        return state with
        {
            IsLoading = false,
            Data = action.Data,
        };
    }

    /// <summary>
    /// On request error reducer
    /// </summary>
    /// <typeparam name="TState">Request state type</typeparam>
    /// <typeparam name="TAction">Request action type</typeparam>
    /// <param name="state">Query state</param>
    /// <param name="action">Query action</param>
    /// <returns>Updated state</returns>
    public static TState OnCommandError<TState, TAction>(this TState state, TAction action)
        where TState : CommandState
        where TAction : IRequestErrorAction
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);

        return state with
        {
            IsSaving = false,
            ErrorMessage = action.ErrorMessage,
        };
    }

    /// <summary>
    /// On query error reducer
    /// </summary>
    /// <typeparam name="TState">Query state type</typeparam>
    /// <typeparam name="TStateData">Query state data type</typeparam>
    /// <typeparam name="TAction">Query action type</typeparam>
    /// <param name="state">Query state</param>
    /// <param name="action">Query action</param>
    /// <returns>Updated state</returns>
    public static TState OnQueryError<TState, TStateData, TAction>(
        this TState state,
        TAction action)
        where TState : QueryState<TStateData>
        where TStateData : class
        where TAction : IRequestErrorAction
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);

        return state with
        {
            IsLoading = false,
            ErrorMessage = action.ErrorMessage,
        };
    }
}
