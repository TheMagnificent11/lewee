using System.Diagnostics.CodeAnalysis;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.Contracts.Orders.Actions;
using Pizzeria.Store.Contracts.Pizzas.Actions;
using Pizzeria.Store.StateManagement.Orders;
using Pizzeria.Store.StateManagement.Pizzas;

namespace Pizzeria.Store.Components;

public partial class Order : FluxorComponent
{
    [Inject]
    private IState<OrdersState> OrdersState { get; set; } = null!;

    [Inject]
    private IState<PizzasState> PizzasState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (this.OrdersState.Value.CurrentOrder == null)
        {
            this.Navigation.NavigateTo(PageRoutes.Home);
            return;
        }

        if (!this.PizzasState.Value.Pizzas.Any() && !this.PizzasState.Value.IsLoading)
        {
            this.Dispatcher.Dispatch(new LoadPizzasAction());
        }
    }

    private void AddPizza(Guid pizzaId)
    {
        if (this.OrdersState.Value.CurrentOrder != null)
        {
            this.Dispatcher.Dispatch(new AddPizzaToOrderAction
            {
                OrderId = this.OrdersState.Value.CurrentOrder.Id,
                PizzaId = pizzaId,
            });
        }
    }

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static members should appear before non-static members", Justification = "Helper method is more readable when placed near its usage context")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter '_' should begin with lower-case letter", Justification = "Underscore is the standard discard pattern for unused parameters")]
    private static void RemovePizza(Guid _)
    {
        // For this demo, we'll implement decreasing quantity as a future enhancement
        // The API only supports adding pizzas, not removing them
    }

    private void ClearError()
    {
        this.Dispatcher.Dispatch(new ClearOrderErrorAction());
    }
}
