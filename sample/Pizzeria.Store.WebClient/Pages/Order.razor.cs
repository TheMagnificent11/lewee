using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Pizzeria.Store.WebClient.States.Orders;
using Pizzeria.Store.WebClient.States.Orders.Actions;
using Pizzeria.Store.WebClient.States.Pizzas;
using Pizzeria.Store.WebClient.States.Pizzas.Actions;

namespace Pizzeria.Store.WebClient.Pages;

public partial class Order : FluxorComponent
{
    [Inject] private IState<OrdersState> OrdersState { get; set; } = null!;
    [Inject] private IState<PizzasState> PizzasState { get; set; } = null!;
    [Inject] private IDispatcher Dispatcher { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        if (OrdersState.Value.CurrentOrderId == null)
        {
            Navigation.NavigateTo("/");
            return;
        }

        if (PizzasState.Value.Pizzas.Length == 0 && !PizzasState.Value.IsLoading)
        {
            Dispatcher.Dispatch(new LoadPizzasAction());
        }
    }

    private void AddPizza(Guid pizzaId)
    {
        if (OrdersState.Value.CurrentOrderId != null)
        {
            Dispatcher.Dispatch(new AddPizzaToOrderAction(OrdersState.Value.CurrentOrderId.Value, pizzaId));
        }
    }

    private static void RemovePizza(Guid _)
    {
        // For this demo, we'll implement decreasing quantity as a future enhancement
        // The API only supports adding pizzas, not removing them
    }

    private void ClearError()
    {
        Dispatcher.Dispatch(new ClearOrderErrorAction());
    }
}