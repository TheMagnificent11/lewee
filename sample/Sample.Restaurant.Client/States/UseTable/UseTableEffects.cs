using Fluxor;
using Lewee.Blazor.ErrorHandling;
using Lewee.Blazor.Fluxor;
using Lewee.Shared;
using Microsoft.AspNetCore.Components;
using Sample.Restaurant.Client.States.TableDetails.Actions;
using Sample.Restaurant.Client.States.UseTable.Actions;

namespace Sample.Restaurant.Client.States.UseTable;

public sealed class UseTableEffects
    : RequestEffects<UseTableState, UseTableAction, UseTableSuccessAction, UseTableErrorAction>
{
    private readonly ITableClient tableClient;
    private readonly NavigationManager navigationManager;

    public UseTableEffects(
        IState<UseTableState> state,
        ITableClient tableClient,
        NavigationManager navigationManager,
        ILogger<UseTableEffects> logger)
        : base(state, logger)
    {
        this.tableClient = tableClient;
        this.navigationManager = navigationManager;
    }

    [EffectMethod]
    public Task MesageReceived(UseTableCompletedAction action, IDispatcher dispatcher)
    {
        using (this.Logger.BeginScope(new Dictionary<string, string>
        {
            { LoggingConsts.CorrelationId, action.CorrelationId.ToString() }
        }))
        {
            this.Logger.LogDebug("Received tabled used message from server");

            dispatcher.Dispatch(new GetTableDetailsAction(action.CorrelationId, action.TableNumber));

            return Task.CompletedTask;
        }
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
            await this.tableClient.UseAsync(action.TableNumber, action.CorrelationId);
        }
        catch (ApiException ex)
        {
            ex.Log(this.Logger);
            dispatcher.Dispatch(new UseTableErrorAction(ex.Message));

            return;
        }

        dispatcher.Dispatch(new UseTableSuccessAction());
    }
}
