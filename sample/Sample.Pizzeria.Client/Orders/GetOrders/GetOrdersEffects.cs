using Correlate;
using Fluxor;
using Lewee.Blazor.Fluxor;
using Lewee.Blazor.Logging;
using Refit;

namespace Sample.Pizzeria.Client.Orders.GetOrders;

public sealed class GetOrdersEffects :
    RequestEffects<GetOrdersState, GetOrdersAction, GetOrdersSuccessAction, GetOrdersErrorAction>
{
    private readonly IOrdersApi ordersApi;

    public GetOrdersEffects(
        IState<GetOrdersState> state,
        IOrdersApi ordersApi,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<GetOrdersEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.ordersApi = ordersApi;
    }

    protected override async Task ExecuteRequest(GetOrdersAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await this.ordersApi.GetUserOrders(CancellationToken.None);
            dispatcher.Dispatch(new GetOrdersSuccessAction(result, action.CorrelationId));
        }
        catch (ApiException ex)
        {
            this.Logger.LogApiException(ex);
            dispatcher.Dispatch(new GetOrdersErrorAction(ex.Message, action.CorrelationId));
        }
    }
}
