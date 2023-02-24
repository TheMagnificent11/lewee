using Fluxor;
using Lewee.Fluxor;
using Lewee.Fluxor.ErrorHandling;
using Sample.Restaurant.App.States.Tables.Actions;

namespace Sample.Restaurant.App.States.Tables;

public sealed class TablesEffects
    : BaseRequestEffects<TablesEffects, TablesState, GetTablesAction, GetTablesSuccessAction, GetTablesErrorAction>
{
    private readonly ITableClient tableClient;

    public TablesEffects(IState<TablesState> state, ITableClient tableClient, Serilog.ILogger logger)
        : base(state, logger)
    {
        this.tableClient = tableClient;
    }

    protected override async Task ExecuteRequest(GetTablesAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await this.tableClient.GetAllAsync(action.CorrelationId);
            dispatcher.Dispatch(new GetTablesSuccessAction(result.ToArray()));
        }
        catch (ApiException ex)
        {
            ex.Log(this.Logger);
            dispatcher.Dispatch(new GetTablesErrorAction(ex.Message));
        }
    }
}
