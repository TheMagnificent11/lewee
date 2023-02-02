using Fluxor;
using Lewee.Fluxor.Actions;
using MediatR;
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
    /// <param name="mediator">Mediator</param>
    /// <param name="logger">Logger</param>
    protected BaseQueryEffects(IState<TState> state, IMediator mediator, ILogger logger)
    {
        this.State = state;
        this.Mediator = mediator;
        this.Logger = logger.ForContext<TEffects>();
    }

    /// <summary>
    /// Gets the state
    /// </summary>
    protected IState<TState> State { get; }

    /// <summary>
    /// Gets the mediator
    /// </summary>
    protected IMediator Mediator { get; }

    /// <summary>
    /// Gets the logger
    /// </summary>
    protected ILogger Logger { get; }

    [EffectMethod]
    public async Task Query(TQueryAction action, IDispatcher dispatcher)
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, action.CorrelationId))
        using (LogContext.PushProperty(RequestType, this.State.Value.RequestType))
        {
            this.logger.Debug("Dispatching request...");

            var result = await this.mediator.Send(new GetTablesQuery(action.CorrelationId));

            if (result.IsSuccess)
            {
                dispatcher.Dispatch(new GetTablesSuccess(result.Data?.ToArray() ?? Array.Empty<TableDto>()));
                return;
            }

            dispatcher.Dispatch(new GetTablesError(result.GenerateErrorMessage()));
        }
    }

    [EffectMethod]
#pragma warning disable IDE0060 // Remove unused parameter (required by Fluxor)
    public Task QuerySucces(TQuerySuccessAction action, IDispatcher dispatcher)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, this.State.Value.CorrelationId))
        using (LogContext.PushProperty(RequestType, this.State.Value.RequestType))
        {
            this.logger.Debug("Dispatching request...success");
            return Task.FromResult(true);
        }
    }

    [EffectMethod]
#pragma warning disable IDE0060 // Remove unused parameter (required by Fluxor)
    public Task QueryError(TQueryErrorAction action, IDispatcher dispatcher)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, this.State.Value.CorrelationId))
        using (LogContext.PushProperty(RequestType, this.State.Value.RequestType))
        {
            this.logger.Error("Dispatching request...error (Error Message: {ErrorMessage})", action.ErrorMessage);
            return Task.FromResult(false);
        }
    }
}
