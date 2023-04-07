using Fluxor;
using Lewee.Blazor.Fluxor.Actions;
using Lewee.Shared;
using Microsoft.Extensions.Logging; // TODO (https://github.com/TheMagnificent11/lewee/issues/15): switch to Serilog

namespace Lewee.Blazor.Fluxor;

/// <summary>
/// Request Effects
/// </summary>
/// <typeparam name="TState">State type</typeparam>
/// <typeparam name="TRequestAction">Request action type</typeparam>
/// <typeparam name="TRequestSuccessAction">Request success action</typeparam>
/// <typeparam name="TRequestErrorAction">Request error action</typeparam>
public abstract class RequestEffects<TState, TRequestAction, TRequestSuccessAction, TRequestErrorAction>

    where TState : RequestState
    where TRequestAction : IRequestAction
    where TRequestSuccessAction : IRequestSuccessAction
    where TRequestErrorAction : IRequestErrorAction
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequestEffects{TState, TRequestAction, TRequestSuccessAction, TRequestErrorAction}"/> class
    /// </summary>
    /// <param name="state">State</param>
    /// <param name="logger">Logger</param>
    public RequestEffects(IState<TState> state, ILogger logger)
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
        using (this.Logger.BeginScope(new Dictionary<string, string>
        {
            { LoggingConsts.CorrelationId, action.CorrelationId.ToString() },
            { LoggingConsts.RequestType, this.State.Value.RequestType }
        }))
        {
            this.Logger.LogDebug("Executing query request...");

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
        using (this.Logger.BeginScope(new Dictionary<string, string>
        {
            { LoggingConsts.CorrelationId, this.State.Value.CorrelationId.ToString() },
            { LoggingConsts.RequestType, this.State.Value.RequestType }
        }))
        {
            this.Logger.LogDebug("Executing query request...success");
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
        using (this.Logger.BeginScope(new Dictionary<string, string>
        {
            { LoggingConsts.CorrelationId, this.State.Value.CorrelationId.ToString() },
            { LoggingConsts.RequestType, this.State.Value.RequestType }
        }))
        {
            this.Logger.LogError("Executing query request...error (Error Message: {ErrorMessage})", action.ErrorMessage);
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
