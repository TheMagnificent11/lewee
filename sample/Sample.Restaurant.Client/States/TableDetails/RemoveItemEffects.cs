using Correlate;
using Fluxor;
using Lewee.Blazor.ErrorHandling;
using Lewee.Blazor.Fluxor;
using Lewee.Blazor.Logging;
using Sample.Restaurant.Client.States.TableDetails.Actions;

namespace Sample.Restaurant.Client.States.TableDetails;

public sealed class RemoveItemEffects
    : RequestEffects<TableDetailsState, RemoveItemAction, RemoveItemSuccessAction, RemoveItemErrorAction>
{
    private readonly ITableClient tableClient;

    public RemoveItemEffects(
        IState<TableDetailsState> state,
        ITableClient tableClient,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<RemoveItemEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.tableClient = tableClient;
    }

    [EffectMethod]
    public Task RemoveItemCompleted(RemoveItemCompletedAction action, IDispatcher dispatcher)
    {
        using (this.Logger.BeginCorrelationIdScope(action.CorrelationId))
        {
            this.Logger.LogDebug("Received item removed message from server");

            dispatcher.Dispatch(new GetTableDetailsAction(action.CorrelationId, action.TableNumber));

            return Task.CompletedTask;
        }
    }

    protected override async Task ExecuteRequest(RemoveItemAction action, IDispatcher dispatcher)
    {
        try
        {
            await this.tableClient.RemoveMenuItemAsync(action.TableNumber, action.MenuItemId);
            dispatcher.Dispatch(new RemoveItemSuccessAction(action.CorrelationId));
        }
        catch (ApiException ex)
        {
            ex.Log(this.Logger);
            dispatcher.Dispatch(new RemoveItemErrorAction(ex.Message, action.CorrelationId));
        }
    }
}
