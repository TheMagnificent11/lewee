using Fluxor;
using Lewee.Fluxor;
using MediatR;
using Sample.Restaurant.App.States.Tables.Actions;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.App.States.Tables;

public sealed class TablesEffects
    : BaseRequestEffects<TablesEffects, TablesState, GetTablesAction, GetTablesSuccessAction, GetTablesErrorAction>
{
    private readonly IMediator mediator;

    public TablesEffects(IState<TablesState> state, IMediator mediator, Serilog.ILogger logger)
        : base(state, logger)
    {
        this.mediator = mediator;
    }

    protected override async Task ExecuteRequest(GetTablesAction action, IDispatcher dispatcher)
    {
        var result = await this.mediator.Send(new GetTablesQuery(action.CorrelationId));

        if (result.IsSuccess && result.Data != null)
        {
            dispatcher.Dispatch(new GetTablesSuccessAction(result.Data.ToArray()));
            return;
        }

        dispatcher.Dispatch(new GetTablesErrorAction(result.GenerateErrorMessage()));
    }
}
