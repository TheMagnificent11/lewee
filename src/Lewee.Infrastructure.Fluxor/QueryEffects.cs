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
public abstract class QueryEffects<TState, TData, TRequestAction, TRequestSuccessAction, TRequestErrorAction>
    : RequestEffects<TState, TData, TRequestSuccessAction, TRequestErrorAction>
    where TState : RequestState<TData>, new()
    where TData : class
    where TRequestAction : IRequestAction, new()
    where TRequestSuccessAction : IQuerySuccessAction<TData>, new()
    where TRequestErrorAction : IRequestErrorAction, new()
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="QueryEffects{TState, TData, TRequestAction, TRequestSuccessAction, TRequestErrorAction}"/> class
    /// </summary>
    /// <param name="state">State</param>
    /// <param name="correlationContextAccessor">Correlation context accessor</param>
    /// <param name="logger">Logger</param>
    protected QueryEffects(
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
    public virtual async Task OnQueryAsync(
        [NotNull] TRequestAction action,
        [NotNull] IDispatcher dispatcher)
    {
        using (this.Logger.BeginCorrelationIdScope(this.CorrelationContextAccessor, action))
        {
            this.Logger.LogDebug("Executing request...");

            try
            {
                var result = await this.ExecuteQueryAsync(action, dispatcher);
                if (result.IsSuccess)
                {
                    dispatcher.Dispatch(new TRequestSuccessAction
                    {
                        CorrelationId = action.CorrelationId,
                        Data = result.Data!,
                    });

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
                this.Logger.LogError(
                    ex,
                    "An error occurred while executing the query request: {ErrorMessage}",
                    ex.Message);

                dispatcher.Dispatch(new TRequestErrorAction
                {
                    CorrelationId = action.CorrelationId,
                    ErrorMessage = ex.Message,
                });
            }
        }
    }

    /// <summary>
    /// Executes the query
    /// </summary>
    /// <param name="action">Request action</param>
    /// <param name="dispatcher">Dispatcher</param>
    /// <returns>Asynchronous task containing a <see cref="Result"/> that represents the success or failure of the request</returns>
    protected abstract Task<QueryResult<TData>> ExecuteQueryAsync(
        [NotNull] TRequestAction action,
        [NotNull] IDispatcher dispatcher);
}
