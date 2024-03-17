using Correlate;
using Fluxor;
using Lewee.Blazor.Fluxor;
using Lewee.Blazor.Logging;
using Refit;

namespace Sample.Pizzeria.Client.State.StartOrder;

public sealed class StartOrderEffects :
    RequestEffects<StartOrderState, StartOrderAction, StartOrderSuccessAction, StartOrderErrorAction>
{
    private readonly IOrdersApi ordersApi;

    public StartOrderEffects(
        IState<StartOrderState> state,
        IOrdersApi ordersApi,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<StartOrderEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.ordersApi = ordersApi;
    }

    // TODO: success action handler to navigate to the next page

    protected override async Task ExecuteRequest(StartOrderAction action, IDispatcher dispatcher)
    {
        try
        {
            await this.ordersApi.StartOrder(CancellationToken.None);
            dispatcher.Dispatch(new StartOrderSuccessAction(action.CorrelationId));
        }
        catch (ApiException ex)
        {
            this.Logger.LogApiException(ex);
            dispatcher.Dispatch(new StartOrderErrorAction("Failed to start order", action.CorrelationId));
        }
    }
}
