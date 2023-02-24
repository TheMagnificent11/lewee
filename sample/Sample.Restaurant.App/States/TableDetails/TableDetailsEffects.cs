using Fluxor;
using Lewee.Fluxor;
using MediatR;
using Microsoft.AspNetCore.Components;
using Sample.Restaurant.App.States.TableDetails.Actions;

namespace Sample.Restaurant.App.States.TableDetails;

public sealed class TableDetailsEffects
    : BaseRequestEffects<TableDetailsEffects, TableDetailsState, GetTableDetailsAction, GetTableDetailsSuccessAction, GetTableDetailsErrorAction>
{
    private readonly ITableClient tableClient;
    private readonly NavigationManager navigationManager;

    public TableDetailsEffects(
        IState<TableDetailsState> state,
        ITableClient tableClient,
        NavigationManager navigationManager,
        Serilog.ILogger logger)
        : base(state, logger)
    {
        this.tableClient = tableClient;
        this.navigationManager = navigationManager;
    }

    [EffectMethod]
#pragma warning disable IDE0060 // Remove unused parameter (required by Fluxor)
    public Task NavigateToTableDetails(GetTableDetailsSuccessAction action, IDispatcher dispatcher)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        this.navigationManager.NavigateTo($"tables/{action.Data.TableNumber}");
        return Task.FromResult(true);
    }

    protected override async Task ExecuteRequest(GetTableDetailsAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await this.tableClient.GetDetailsAsync(action.TableNumber, action.CorrelationId);
            dispatcher.Dispatch(new GetTableDetailsSuccessAction(result));
        }
        catch (ApiException ex)
        {
            // TODO: logging
            dispatcher.Dispatch(new GetTableDetailsErrorAction(ex.Message));
        }
    }
}
