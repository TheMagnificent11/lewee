namespace Lewee.Infrastructure.Fluxor;

/// <summary>
/// Reducer Extension Methods
/// </summary>
public static class ReducerExtensions
{
    /// <summary>
    /// On command reducer
    /// </summary>
    /// <typeparam name="TState">State type</typeparam>
    /// <typeparam name="TData">Data type</typeparam>
    /// <typeparam name="TAction">Request action type</typeparam>
    /// <param name="state">Requet state</param>
    /// <param name="action">Request action</param>
    /// <param name="clearData">Whether to clear the state data property</param>
    /// <returns>Updated state</returns>
    public static TState OnCommand<TState, TData, TAction>(this TState state, TAction action, bool clearData)
        where TState : RequestState<TData>
        where TData : class
        where TAction : IRequestAction
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);

        return state with
        {
            IsSaving = true,
            CorrelationId = action.CorrelationId,
            ErrorMessage = null,
            Data = clearData ? null : state.Data,
        };
    }

    /// <summary>
    /// On query reducer
    /// </summary>
    /// <typeparam name="TState">State type</typeparam>
    /// <typeparam name="TData">Data type</typeparam>
    /// <typeparam name="TAction">Query action type</typeparam>
    /// <param name="state">Query state</param>
    /// <param name="action">Query action</param>
    /// <returns>Updated state</returns>
    public static TState OnQuery<TState, TData, TAction>(this TState state, TAction action)
        where TState : RequestState<TData>
        where TData : class
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
    /// <typeparam name="TState">State type</typeparam>
    /// <typeparam name="TData">Data type</typeparam>
    /// <typeparam name="TAction">Request action type</typeparam>
    /// <param name="state">Requet state</param>
    /// <param name="action">Success action</param>
    /// <returns>Updated state</returns>
    public static TState OnCommandSuccess<TState, TData, TAction>(this TState state, TAction action)
        where TState : RequestState<TData>
        where TData : class
        where TAction : IRequestSuccessAction
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);

        return state with
        {
            CorrelationId = action.CorrelationId,
            IsSaving = false,
            ErrorMessage = null,
        };
    }

    /// <summary>
    /// On query success reducer
    /// </summary>
    /// <typeparam name="TState">Query state type</typeparam>
    /// <typeparam name="TData">Data type</typeparam>
    /// <typeparam name="TAction">Query action type</typeparam>
    /// <param name="state">Query state</param>
    /// <param name="action">Query action</param>
    /// <returns>Updated state</returns>
    public static TState OnQuerySuccess<TState, TData, TAction>(
        this TState state,
        TAction action)
        where TState : RequestState<TData>
        where TData : class
        where TAction : IQuerySuccessAction<TData>
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);

        return state with
        {
            CorrelationId = action.CorrelationId,
            IsLoading = false,
            Data = action.Data,
        };
    }

    /// <summary>
    /// On request error reducer
    /// </summary>
    /// <typeparam name="TState">State type</typeparam>
    /// <typeparam name="TData">Data type</typeparam>
    /// <typeparam name="TAction">Request action type</typeparam>
    /// <param name="state">Query state</param>
    /// <param name="action">Error action</param>
    /// <returns>Updated state</returns>
    public static TState OnCommandError<TState, TData, TAction>(this TState state, TAction action)
        where TState : RequestState<TData>
        where TData : class
        where TAction : IRequestErrorAction
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);

        return state with
        {
            CorrelationId = action.CorrelationId,
            IsSaving = false,
            ErrorMessage = action.ErrorMessage,
        };
    }

    /// <summary>
    /// On query error reducer
    /// </summary>
    /// <typeparam name="TState">State type</typeparam>
    /// <typeparam name="TData">Data type</typeparam>
    /// <typeparam name="TAction">Error action type</typeparam>
    /// <param name="state">Query state</param>
    /// <param name="action">Query action</param>
    /// <returns>Updated state</returns>
    public static TState OnQueryError<TState, TData, TAction>(
        this TState state,
        TAction action)
        where TState : RequestState<TData>
        where TData : class
        where TAction : IRequestErrorAction
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);

        return state with
        {
            CorrelationId = action.CorrelationId,
            IsLoading = false,
            ErrorMessage = action.ErrorMessage,
        };
    }

    /// <summary>
    /// On command completed reducer
    /// </summary>
    /// <typeparam name="TState">State type</typeparam>
    /// <typeparam name="TData">Data type</typeparam>
    /// <typeparam name="TAction">Error action type</typeparam>
    /// <param name="state">Query state</param>
    /// <param name="action">Query action</param>
    /// <returns>Updated state</returns>
    public static TState OnCommandCompleted<TState, TData, TAction>(
        this TState state,
        TAction action)
        where TState : RequestState<TData>
        where TData : class
        where TAction : IMessageReceivedAction<TData>
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);

        return state with
        {
            CorrelationId = action.CorrelationId,
            Data = action.Data,
        };
    }
}
