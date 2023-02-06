using Fluxor;
using Lewee.Fluxor.Actions;
using Lewee.Shared;
using Serilog;
using Serilog.Context;

namespace Lewee.Fluxor;

/// <summary>
/// Base Request Effects
/// </summary>
/// <typeparam name="TEffects">Type that inherits this base class</typeparam>
/// <typeparam name="TState">State type</typeparam>
/// <typeparam name="TRequestAction">Request action type</typeparam>
/// <typeparam name="TRequestSuccessAction">Request success action</typeparam>
/// <typeparam name="TRequestErrorAction">Request error action</typeparam>
public abstract class BaseRequestEffects<TEffects, TState, TRequestAction, TRequestSuccessAction, TRequestErrorAction>
    where TEffects : class
    where TState : BaseRequestState
    where TRequestAction : IRequestAction
    where TRequestSuccessAction : IRequestSuccessAction
    where TRequestErrorAction : IRequestErrorAction
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRequestEffects{TEffects, TState, TRequestAction, TRequestSuccessAction, TRequestErrorAction}"/> class
    /// </summary>
    /// <param name="state">State</param>
    /// <param name="logger">Logger</param>
    public BaseRequestEffects(IState<TState> state, ILogger logger)
    {
        this.State = state;
        this.Logger = logger;
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
    /// Request effect
    /// </summary>
    /// <param name="action">Action</param>
    /// <param name="dispatcher">Dispatcher</param>
    /// <returns>Asynchronous task</returns>
    [EffectMethod]
    public virtual async Task Query(TRequestAction action, IDispatcher dispatcher)
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, action.CorrelationId))
        using (LogContext.PushProperty(LoggingConsts.RequestType, this.State.Value.RequestType))
        {
            this.Logger.Debug("Dispatching request...");

            await this.ExecuteRequest(action, dispatcher);
        }
    }

    /// <summary>
    /// Request success effect
    /// </summary>
    /// <param name="action">Action</param>
    /// <param name="dispatcher">Dispatcher</param>
    /// <returns>Asynchronous task</returns>
    [EffectMethod]
    public virtual Task QuerySucces(TRequestSuccessAction action, IDispatcher dispatcher)
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, this.State.Value.CorrelationId))
        using (LogContext.PushProperty(LoggingConsts.RequestType, this.State.Value.RequestType))
        {
            this.Logger.Debug("Dispatching request...success");
            return Task.FromResult(true);
        }
    }

    /// <summary>
    /// Request error effect
    /// </summary>
    /// <param name="action">Action</param>
    /// <param name="dispatcher">Dispatcher</param>
    /// <returns>Asynchronous task</returns>
    [EffectMethod]
    public virtual Task QueryError(TRequestErrorAction action, IDispatcher dispatcher)
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, this.State.Value.CorrelationId))
        using (LogContext.PushProperty(LoggingConsts.RequestType, this.State.Value.RequestType))
        {
            this.Logger.Error("Dispatching request...error (Error Message: {ErrorMessage})", action.ErrorMessage);
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Executes the request
    /// </summary>
    /// <param name="action">Request action</param>
    /// <param name="dispatcher">Dispatcher</param>
    /// <returns>Asynchronous task</returns>
    protected abstract Task ExecuteRequest(TRequestAction action, IDispatcher dispatcher);
}
