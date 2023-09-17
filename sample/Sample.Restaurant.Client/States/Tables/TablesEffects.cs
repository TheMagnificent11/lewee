using Correlate;
using Fluxor;
using Lewee.Blazor.ErrorHandling;
using Lewee.Blazor.Fluxor;
using Sample.Restaurant.Client.States.Tables.Actions;

namespace Sample.Restaurant.Client.States.Tables;

public sealed class TablesEffects
    : RequestEffects<TablesState, GetTablesAction, GetTablesSuccessAction, GetTablesErrorAction>
{
    private readonly ITableClient tableClient;

    public TablesEffects(
        IState<TablesState> state,
        ITableClient tableClient,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<TablesEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.tableClient = tableClient;
    }

    protected override async Task ExecuteRequest(GetTablesAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await this.tableClient.GetAllAsync(action.CorrelationId);
            dispatcher.Dispatch(new GetTablesSuccessAction(result.ToArray(), action.CorrelationId));
        }
        catch (ApiException ex)
        {
            ex.Log(this.Logger);
            dispatcher.Dispatch(new GetTablesErrorAction(ex.Message, action.CorrelationId));
        }
    }
}
