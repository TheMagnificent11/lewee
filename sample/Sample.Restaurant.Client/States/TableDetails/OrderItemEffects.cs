using Correlate;
using Fluxor;
using Lewee.Blazor.ErrorHandling;
using Lewee.Blazor.Fluxor;
using Lewee.Blazor.Logging;
using Sample.Restaurant.Client.States.TableDetails.Actions;

namespace Sample.Restaurant.Client.States.TableDetails;

public sealed class OrderItemEffects
    : RequestEffects<TableDetailsState, OrderItemAction, OrderItemSuccessAction, OrderItemErrorAction>
{
    private readonly ITableClient tableClient;

    public OrderItemEffects(
        IState<TableDetailsState> state,
        ITableClient tableClient,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<OrderItemEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.tableClient = tableClient;
    }

    [EffectMethod]
    public Task OrderItemCompleted(OrderItemCompletedAction action, IDispatcher dispatcher)
    {
        using (this.Logger.BeginCorrelationIdScope(action.CorrelationId))
        {
            this.Logger.LogDebug("Received item added message from server");

            dispatcher.Dispatch(new GetTableDetailsAction(action.CorrelationId, action.TableNumber));

            return Task.CompletedTask;
        }
    }

    protected override async Task ExecuteRequest(OrderItemAction action, IDispatcher dispatcher)
    {
        try
        {
            await this.tableClient.OrderMenuItemAsync(action.TableNumber, action.MenuItemId);
            dispatcher.Dispatch(new OrderItemSuccessAction(action.CorrelationId));
        }
        catch (ApiException ex)
        {
            ex.Log(this.Logger);
            dispatcher.Dispatch(new OrderItemErrorAction(ex.Message, action.CorrelationId));
        }
    }
}
