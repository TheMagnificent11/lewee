using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.Common;
using Lewee.StateManagement.Observability;
using Microsoft.Extensions.Logging;

namespace Lewee.StateManagement;

/// <summary>
/// Request Effects
/// </summary>
/// <typeparam name="TState">State type</typeparam>
/// <typeparam name="TRequestAction">Request action type</typeparam>
/// <typeparam name="TRequestSuccessAction">Request success action type</typeparam>
/// <typeparam name="TRequestErrorAction">Request error action type</typeparam>
/// <typeparam name="TMessageReceived">Request completed action type</typeparam>
public abstract class RequestEffects<TState, TRequestAction, TRequestSuccessAction, TRequestErrorAction, TMessageReceived>
    where TState : RequestState, new()
    where TRequestAction : IRequestAction, new()
    where TRequestSuccessAction : IRequestSuccessAction, new()
    where TRequestErrorAction : IRequestErrorAction, new()
    where TMessageReceived : IMessageReceivedAction, new()
{
    private readonly ICorrelationContextAccessor correlationContextAccessor;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="RequestEffects{TState, TRequestAction, TRequestSuccessAction, TRequestErrorAction, TMessageReceived}"/> class
    /// </summary>
    /// <param name="state">State</param>
    /// <param name="correlationContextAccessor">Correlation context accessor</param>
    /// <param name="logger">Logger</param>
    protected RequestEffects(
        IState<TState> state,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger logger)
    {
        this.State = state;
        this.correlationContextAccessor = correlationContextAccessor;
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
    public virtual async Task RequestAsync(TRequestAction action, IDispatcher dispatcher)
    {
        using (this.Logger.BeginCorrelationIdScope(this.correlationContextAccessor.SetNewCorrelationId(action)))
        {
            this.Logger.LogDebug("Executing query request...");

            try
            {
                var result = await this.ExecuteRequestAsync(action, dispatcher);
                if (result.IsSuccess)
                {
                    dispatcher.Dispatch(new TRequestSuccessAction() { CorrelationId = action.CorrelationId });

                    return;
                }

                dispatcher.Dispatch(new TRequestErrorAction()
                {
                    CorrelationId = action.CorrelationId,
                    ErrorMessage = "Request execution returned failure.",
                });
            }
            catch (Exception ex)
            {
                this.Logger.LogError(
                    ex,
                    "An error occurred while executing the query request: {ErrorMessage}",
                    ex.Message);
            }
        }
    }

    /// <summary>
    /// Request success effect
    /// </summary>
    /// <param name="action">Action</param>
    /// <param name="dispatcher">Dispatcher</param>
    /// <returns>Asynchronous task</returns>
    [EffectMethod]
    public virtual Task RequestSuccessAsync(TRequestSuccessAction action, IDispatcher dispatcher)
    {
        using (this.Logger.BeginCorrelationIdScope(action.CorrelationId))
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
    public virtual Task RequestErrorAsync(TRequestErrorAction action, IDispatcher dispatcher)
    {
        using (this.Logger.BeginCorrelationIdScope(action.CorrelationId))
        {
            this.Logger.LogError(
                "Executing query request...error (Error Message: {ErrorMessage})",
                action.ErrorMessage);

            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Request completed effect
    /// </summary>
    /// <param name="action">Action</param>
    /// <param name="dispatcher">Dispatcher</param>
    /// <returns>Asynchronous task</returns>
    [EffectMethod]
    public virtual async Task RequestCompletedAsync(TMessageReceived action, IDispatcher dispatcher)
    {
        using (this.Logger.BeginCorrelationIdScope(action.CorrelationId))
        {
            await this.ExecuteRequestCompletedAsync(action, dispatcher);

            this.Logger.LogDebug("Executing query request...completed");
        }
    }

    /// <summary>
    /// Executes the request
    /// </summary>
    /// <param name="action">Request action</param>
    /// <param name="dispatcher">Dispatcher</param>
    /// <returns>Asynchronous task containing a <see cref="Result"/> that represents the success or failure of the request</returns>
    protected abstract Task<Result> ExecuteRequestAsync(
        [NotNull] TRequestAction action,
        [NotNull] IDispatcher dispatcher);

    /// <summary>
    /// Executes the request completed
    /// </summary>
    /// <param name="action">Request completed action</param>
    /// <param name="dispatcher">Dispatcher</param>
    /// <returns>Asynchronous task</returns>
    protected abstract Task ExecuteRequestCompletedAsync(
        [NotNull] TMessageReceived action,
        [NotNull] IDispatcher dispatcher);
}
