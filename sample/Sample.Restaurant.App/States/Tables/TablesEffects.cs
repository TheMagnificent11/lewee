using Fluxor;
using Lewee.Fluxor;
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
            var result = await this.tableClient.GetAllAsync();
            dispatcher.Dispatch(new GetTablesSuccessAction(result.ToArray()));
        }
        catch (ApiException ex)
        {
            // TODO: logging
            dispatcher.Dispatch(new GetTablesErrorAction(ex.Message));
        }
    }
}
