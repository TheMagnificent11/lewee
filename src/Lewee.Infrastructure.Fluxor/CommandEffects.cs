using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.Common;
using Lewee.Infrastructure.Fluxor.Observability;
using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.Fluxor;

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
    : RequestEffects<TState, TData, TRequestSuccessAction, TRequestErrorAction>
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
    /// Command effect
    /// </summary>
    /// <param name="action">Action</param>
    /// <param name="dispatcher">Dispatcher</param>
    /// <returns>Asynchronous task</returns>
    [EffectMethod]
    public virtual async Task OnCommandAsync(
        [NotNull] TRequestAction action,
        [NotNull] IDispatcher dispatcher)
    {
        using (this.Logger.BeginCorrelationIdScope(this.CorrelationContextAccessor, action))
        {
            this.Logger.LogRequestExecuting();

            try
            {
                var result = await this.ExecuteCommandAsync(action, dispatcher);
                if (result.IsSuccess)
                {
                    dispatcher.Dispatch(new TRequestSuccessAction() { CorrelationId = action.CorrelationId });

                    return;
                }

                dispatcher.Dispatch(new TRequestErrorAction()
                {
                    CorrelationId = action.CorrelationId,
                    ErrorMessage = result.GenerateErrorMessage(),
                });
            }
            catch (Exception ex)
            {
                this.Logger.LogRequestExecutionFailed(ex, ex.Message);

                dispatcher.Dispatch(new TRequestErrorAction
                {
                    CorrelationId = action.CorrelationId,
                    ErrorMessage = ex.Message,
                });
            }
        }
    }

    /// <summary>
    /// Command completed effect
    /// </summary>
    /// <param name="action">Action</param>
    /// <param name="dispatcher">Dispatcher</param>
    /// <returns>Asynchronous task</returns>
    [EffectMethod]
    public virtual async Task OnCommandCompletedAsync(
        [NotNull] TMessageReceived action,
        [NotNull] IDispatcher dispatcher)
    {
        using (this.Logger.BeginCorrelationIdScope(this.CorrelationContextAccessor, action))
        {
            await this.ExecuteCommandCompletedAsync(action, dispatcher);

            this.Logger.LogQueryRequestCompleted();
        }
    }

    /// <summary>
    /// Executes the command
    /// </summary>
    /// <param name="action">Request action</param>
    /// <param name="dispatcher">Dispatcher</param>
    /// <returns>Asynchronous task containing a <see cref="Result"/> that represents the success or failure of the request</returns>
    protected abstract Task<CommandResult> ExecuteCommandAsync(
        [NotNull] TRequestAction action,
        [NotNull] IDispatcher dispatcher);

    /// <summary>
    /// Executes the command completed
    /// </summary>
    /// <param name="action">Command completed action</param>
    /// <param name="dispatcher">Dispatcher</param>
    /// <returns>Asynchronous task</returns>
    protected abstract Task ExecuteCommandCompletedAsync(
        [NotNull] TMessageReceived action,
        [NotNull] IDispatcher dispatcher);
}
