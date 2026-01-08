using System.Diagnostics.CodeAnalysis;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.StateManagement.Orders;
using Pizzeria.Store.StateManagement.Orders.Actions;
using Pizzeria.Store.StateManagement.Pizzas;
using Pizzeria.Store.StateManagement.Pizzas.Actions;

namespace Pizzeria.Store.Components;

public partial class Order : FluxorComponent
{
    [Parameter]
    public Guid OrderId { get; set; }

    [Inject]
    private IState<OrderState> OrdersState { get; set; } = null!;

    [Inject]
    private IState<PizzasState> PizzasState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (this.OrdersState.Value.Data == null)
        {
            this.Navigation.NavigateTo(PageRoutes.Home);
            return;
        }

        if (this.PizzasState.Value.Data == null && !this.PizzasState.Value.IsLoading)
        {
            this.Dispatcher.Dispatch(new LoadPizzasAction());
        }
    }

    private void AddPizza(Guid pizzaId)
    {
        if (this.OrdersState.Value.Data != null)
        {
            this.Dispatcher.Dispatch(new AddPizzaToOrderAction
            {
                OrderId = this.OrdersState.Value.Data.Id,
                PizzaId = pizzaId,
            });
        }
    }

    [SuppressMessage(
        "StyleCop.CSharp.OrderingRules",
        "SA1204:Static members should appear before non-static members",
        Justification = "Helper method is more readable when placed near its usage context")]
    [SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1313:Parameter '_' should begin with lower-case letter",
        Justification = "Underscore is the standard discard pattern for unused parameters")]
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
