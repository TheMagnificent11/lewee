using Fluxor;
using Lewee.Fluxor.Actions;
using Lewee.Shared;
using Serilog;
using Serilog.Context;

namespace Lewee.Fluxor;

/// <summary>
/// Base Query Effects
/// </summary>
/// <typeparam name="TEffects">Type that inherits this base class</typeparam>
/// <typeparam name="TState">State type</typeparam>
/// <typeparam name="TData">Query data type</typeparam>
/// <typeparam name="TQueryAction">Query action</typeparam>
/// <typeparam name="TQuerySuccessAction">Query success action</typeparam>
/// <typeparam name="TQueryErrorAction">Query error action</typeparam>
public abstract class BaseQueryEffects<TEffects, TState, TData, TQueryAction, TQuerySuccessAction, TQueryErrorAction>
    where TEffects : class
    where TState : BaseQueryState<TData>
    where TData : class
    where TQueryAction : IRequestAction
    where TQuerySuccessAction : IQuerySuccessAction<TData>
    where TQueryErrorAction : IRequestErrorAction
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseQueryEffects{TEffects, TState, TData, TQueryAction, TQuerySuccessAction, TQueryErrorAction}"/> class
    /// </summary>
    /// <param name="state">State</param>
    /// <param name="logger">Logger</param>
    protected BaseQueryEffects(IState<TState> state, ILogger logger)
    {
        this.State = state;
        this.Logger = logger.ForContext<TEffects>();
    }

    /// <summary>
    /// Gets the state
    /// </summary>
    protected IState<TState> State { get; }

    /// <summary>
    /// Gets the logger
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Query effect
    /// </summary>
    /// <param name="action">Action</param>
    /// <param name="dispatcher">Dispatcher</param>
    /// <returns>Asynchronous task</returns>
    [EffectMethod]
    public virtual async Task Query(TQueryAction action, IDispatcher dispatcher)
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, action.CorrelationId))
        using (LogContext.PushProperty(LoggingConsts.RequestType, this.State.Value.RequestType))
        {
            this.Logger.Debug("Dispatching request...");

            await this.ExecuteQuery(action, dispatcher);
        }
    }

    /// <summary>
    /// Query success effect
    /// </summary>
    /// <param name="action">Action</param>
    /// <param name="dispatcher">Dispatcher</param>
    /// <returns>Asynchronous task</returns>
    [EffectMethod]
    public virtual Task QuerySucces(TQuerySuccessAction action, IDispatcher dispatcher)
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, this.State.Value.CorrelationId))
        using (LogContext.PushProperty(LoggingConsts.RequestType, this.State.Value.RequestType))
        {
            this.Logger.Debug("Dispatching request...success");
            return Task.FromResult(true);
        }
    }

    /// <summary>
    /// Query error effect
    /// </summary>
    /// <param name="action">Action</param>
    /// <param name="dispatcher">Dispatcher</param>
    /// <returns>Asynchronous task</returns>
    [EffectMethod]
    public virtual Task QueryError(TQueryErrorAction action, IDispatcher dispatcher)
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, this.State.Value.CorrelationId))
        using (LogContext.PushProperty(LoggingConsts.RequestType, this.State.Value.RequestType))
        {
            this.Logger.Error("Dispatching request...error (Error Message: {ErrorMessage})", action.ErrorMessage);
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Executes the query
    /// </summary>
    /// <param name="action">Query action</param>
    /// <param name="dispatcher">Dispatcher</param>
    /// <returns>Asynchronous task</returns>
    protected abstract Task ExecuteQuery(TQueryAction action, IDispatcher dispatcher);
}
