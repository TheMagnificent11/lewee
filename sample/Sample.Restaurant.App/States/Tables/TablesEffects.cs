using Fluxor;
using Lewee.Application.Mediation;
using MediatR;
using Sample.Restaurant.App.States.Tables.Actions;
using Sample.Restaurant.Application;
using Serilog.Context;

namespace Sample.Restaurant.App.States.Tables;

public sealed class TablesEffects
{
    private const string RequestType = "RequestType";

    private readonly IState<TablesState> state;
    private readonly IMediator mediator;
    private readonly Serilog.ILogger logger;

    public TablesEffects(IState<TablesState> state, IMediator mediator, Serilog.ILogger logger)
    {
        this.state = state;
        this.mediator = mediator;
        this.logger = logger.ForContext<TablesEffects>();
    }

    [EffectMethod]
    public async Task GetTables(GetTables action, IDispatcher dispatcher)
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, action.CorrelationId))
        using (LogContext.PushProperty(RequestType, this.state.Value.RequestType))
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
    public Task GetTablesSuccess(GetTablesSuccess action, IDispatcher dispatcher)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, this.state.Value.CorrelationId))
        using (LogContext.PushProperty(RequestType, this.state.Value.RequestType))
        {
            this.logger.Debug("Dispatching request...success");
            return Task.FromResult(true);
        }
    }

    [EffectMethod]
#pragma warning disable IDE0060 // Remove unused parameter (required by Fluxor)
    public Task GetTablesError(GetTablesError action, IDispatcher dispatcher)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, this.state.Value.CorrelationId))
        using (LogContext.PushProperty(RequestType, this.state.Value.RequestType))
        {
            this.logger.Error("Dispatching request...error (Error Message: {ErrorMessage})", action.ErrorMessage);
            return Task.FromResult(false);
        }
    }
}
