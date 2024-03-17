using Correlate;
using Fluxor;
using Lewee.Blazor.Fluxor;
using Microsoft.AspNetCore.Components;
using Sample.Restaurant.Client.ApiClients;
using Sample.Restaurant.Client.States.TableDetails.Actions;

namespace Sample.Restaurant.Client.States.TableDetails;

public sealed class GetTableDetailsEffects
    : RequestEffects<TableDetailsState, GetTableDetailsAction, GetTableDetailsSuccessAction, GetTableDetailsErrorAction>
{
    private readonly ITableClient tableClient;
    private readonly NavigationManager navigationManager;

    public GetTableDetailsEffects(
        IState<TableDetailsState> state,
        ITableClient tableClient,
        NavigationManager navigationManager,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<GetTableDetailsEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.tableClient = tableClient;
        this.navigationManager = navigationManager;
    }

#pragma warning disable IDE0060 // Remove unused parameter (required by Fluxor)
    [EffectMethod]
    public Task NavigateToTableDetails(GetTableDetailsSuccessAction action, IDispatcher dispatcher)
    {
        this.navigationManager.NavigateTo($"tables/{action.Data.TableNumber}");
        return Task.FromResult(true);
    }
#pragma warning restore IDE0060 // Remove unused parameter

    protected override async Task ExecuteRequest(GetTableDetailsAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await this.tableClient.GetDetailsAsync(action.TableNumber);
            dispatcher.Dispatch(new GetTableDetailsSuccessAction(result, action.CorrelationId));
        }
        catch (ApiException ex)
        {
            ex.Log(this.Logger);
            dispatcher.Dispatch(new GetTableDetailsErrorAction(ex.Message, action.CorrelationId));
        }
    }
}
