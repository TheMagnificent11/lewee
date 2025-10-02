using System.Diagnostics.CodeAnalysis;
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
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter '_' should begin with lower-case letter", Justification = "Underscore is the standard discard pattern for unused parameters")]
    public async Task OnStartOrderAsync(StartOrderAction _, IDispatcher dispatcher)
    {
        try
        {
            await this.apiClient.StartOrderAsync();

            // For demo purposes, generate a random order ID since the API doesn't return one
            var orderId = Guid.NewGuid();
            dispatcher.Dispatch(new StartOrderSuccessAction(orderId));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new StartOrderFailureAction($"Failed to start order: {ex.Message}"));
        }
    }

    [EffectMethod]
    public async Task OnAddPizzaToOrderAsync(AddPizzaToOrderAction action, IDispatcher dispatcher)
    {
        try
        {
            await this.apiClient.AddPizzaToOrderAsync(action.OrderId, action.PizzaId);
            dispatcher.Dispatch(new AddPizzaToOrderSuccessAction(action.PizzaId));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new AddPizzaToOrderFailureAction(action.PizzaId, $"Failed to add pizza: {ex.Message}"));
        }
    }
}
