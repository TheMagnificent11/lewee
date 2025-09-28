using Fluxor;
using Pizzeria.Store.WebClient.Services;
using Pizzeria.Store.WebClient.States.Orders.Actions;

namespace Pizzeria.Store.WebClient.States.Orders;

public class OrdersEffects
{
    private readonly IPizzeriaApiClient apiClient;

    public OrdersEffects(IPizzeriaApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    [EffectMethod]
    public async Task OnStartOrder(StartOrderAction _, IDispatcher dispatcher)
    {
        var result = await this.apiClient.StartOrderAsync();

        if (result.IsSuccess)
        {
            // For demo purposes, generate a random order ID since the API doesn't return one
            var orderId = Guid.NewGuid();
            dispatcher.Dispatch(new StartOrderSuccessAction(orderId));
        }
        else
        {
            dispatcher.Dispatch(new StartOrderFailureAction(result.ErrorMessage ?? "Unknown error occurred"));
        }
    }

    [EffectMethod]
    public async Task OnAddPizzaToOrder(AddPizzaToOrderAction action, IDispatcher dispatcher)
    {
        var result = await this.apiClient.AddPizzaToOrderAsync(action.OrderId, action.PizzaId);

        if (result.IsSuccess)
        {
            dispatcher.Dispatch(new AddPizzaToOrderSuccessAction(action.PizzaId));
        }
        else
        {
            dispatcher.Dispatch(new AddPizzaToOrderFailureAction(action.PizzaId, result.ErrorMessage ?? "Unknown error occurred"));
        }
    }
}