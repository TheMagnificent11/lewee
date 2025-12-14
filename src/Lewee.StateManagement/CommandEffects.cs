using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.StateManagement.Observability;
using Microsoft.Extensions.Logging;

namespace Lewee.StateManagement;

/// <summary>
/// Request Effects
/// </summary>
/// <typeparam name="TState">State type</typeparam>
/// <typeparam name="TData">Data type</typeparam>
/// <typeparam name="TRequestAction">Request action type</typeparam>
/// <typeparam name="TRequestSuccessAction">Request success action type</typeparam>
/// <typeparam name="TRequestErrorAction">Request error action type</typeparam>
/// <typeparam name="TMessageReceived">Request completed action type</typeparam>
public abstract class CommandEffects<TState, TData, TRequestAction, TRequestSuccessAction, TRequestErrorAction, TMessageReceived>
    : RequestEffects<TState, TData, TRequestAction, TRequestSuccessAction, TRequestErrorAction>
    where TState : RequestState<TData>, new()
    where TData : class
    where TRequestAction : IRequestAction, new()
    where TRequestSuccessAction : IRequestSuccessAction, new()
    where TRequestErrorAction : IRequestErrorAction, new()
    where TMessageReceived : IMessageReceivedAction<TData>, new()
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="CommandEffects{TState, TData, TRequestAction, TRequestSuccessAction, TRequestErrorAction, TMessageReceived}"/> class
    /// </summary>
    /// <param name="state">State</param>
    /// <param name="correlationContextAccessor">Correlation context accessor</param>
    /// <param name="logger">Logger</param>
    protected CommandEffects(
        IState<TState> state,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger logger)
        : base(state, correlationContextAccessor, logger)
    {
    }

    /// <summary>
    /// Request completed effect
    /// </summary>
    /// <param name="action">Action</param>
    /// <param name="dispatcher">Dispatcher</param>
    /// <returns>Asynchronous task</returns>
    [EffectMethod]
    public virtual async Task RequestCompletedAsync(
        [NotNull] TMessageReceived action,
        [NotNull] IDispatcher dispatcher)
    {
        using (this.Logger.BeginCorrelationIdScope(action.CorrelationId))
        {
            await this.ExecuteRequestCompletedAsync(action, dispatcher);

            this.Logger.LogDebug("Executing query request...completed");
        }
    }

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
