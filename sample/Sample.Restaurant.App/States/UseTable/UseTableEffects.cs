using Fluxor;
using Lewee.Fluxor;
using MediatR;
using Microsoft.AspNetCore.Components;
using Sample.Restaurant.App.States.UseTable.Actions;

namespace Sample.Restaurant.App.States.UseTable;

public sealed class UseTableEffects
    : BaseRequestEffects<UseTableEffects, UseTableState, UseTableAction, UseTableSuccessAction, UseTableErrorAction>
{
    private readonly ITableClient tableClient;
    private readonly NavigationManager navigationManager;

    public UseTableEffects(
        IState<UseTableState> state,
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
    public Task NavigateToTableDetails(UseTableSuccessAction action, IDispatcher dispatcher)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        this.navigationManager.NavigateTo($"tables/{this.State.Value.TableNumber}");
        return Task.FromResult(true);
    }

    protected override async Task ExecuteRequest(UseTableAction action, IDispatcher dispatcher)
    {
        try
        {
            await this.tableClient.UseAsync(action.TableNumber);
        }
        catch (ApiException ex)
        {
            // TODO: error handling an appropriate error logging accoriding to status code in extension method for ApiException
            ////switch (ex.StatusCode)
            ////{

            ////}

            dispatcher.Dispatch(new UseTableErrorAction(ex.Message));
            return;
        }

        dispatcher.Dispatch(new UseTableSuccessAction());
    }
}
