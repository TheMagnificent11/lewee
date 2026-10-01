using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Pizzeria.Ordering.StateManagement.Orders;
using Pizzeria.Ordering.StateManagement.Orders.Actions;
using Pizzeria.Ordering.StateManagement.Pizzas;
using Pizzeria.Ordering.StateManagement.Pizzas.Actions;
using Pizzeria.Store.Contracts;

namespace Pizzeria.Ordering.Components;

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

    private void RemovePizza(Guid pizzaId)
    {
        if (this.OrdersState.Value.Data != null)
        {
            this.Dispatcher.Dispatch(new RemovePizzaFromOrderAction
            {
                OrderId = this.OrdersState.Value.Data.Id,
                PizzaId = pizzaId,
            });
        }
    }

    private void GoToCheckout()
    {
        if (this.OrdersState.Value.Data != null)
        {
            this.Navigation.NavigateTo(PageRoutes.GetCheckoutRoute(this.OrdersState.Value.Data.Id));
        }
    }

    private void ClearError()
    {
        this.Dispatcher.Dispatch(new ClearOrderErrorAction());
    }

    public static class Selectors
    {
        public const string PizzaMenuHeading = "[role='heading'][aria-level='4']";
        public const string CheckoutButton = $"[role='button'][aria-label='{AriaLabels.Checkout}']";
        public const string OrderStatus = "[role='status']";
    }

    private static class AriaLabels
    {
        public const string Checkout = "checkout";
    }
}
