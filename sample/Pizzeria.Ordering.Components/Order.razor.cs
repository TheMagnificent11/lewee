using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Pizzeria.Ordering.StateManagement.Orders;
using Pizzeria.Ordering.StateManagement.Orders.Actions;
using Pizzeria.Ordering.StateManagement.Pizzas;
using Pizzeria.Ordering.StateManagement.Pizzas.Actions;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.Contracts.Orders;

namespace Pizzeria.Ordering.Components;

public partial class Order : FluxorComponent
{
    private bool isCheckingOut;
    private FulfillmentMethod fulfillmentMethod = FulfillmentMethod.Pickup;
    private string deliveryAddress = string.Empty;
    private string? validationErrorMessage;

    public enum FulfillmentMethod
    {
        Pickup,
        Delivery,
    }

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
        var order = this.OrdersState.Value.Data;
        if (order != null && !this.OrdersState.Value.HasPendingCommands)
        {
            var quantity = order.Pizzas.FirstOrDefault(pizza => pizza.PizzaId == pizzaId)?.Quantity ?? 0;
            this.Dispatcher.Dispatch(new AddPizzaToOrderAction
            {
                OrderId = order.Id,
                PizzaId = pizzaId,
                Quantity = quantity + 1,
            });
        }
    }

    private void RemovePizza(Guid pizzaId)
    {
        var order = this.OrdersState.Value.Data;
        if (order != null && !this.OrdersState.Value.HasPendingCommands)
        {
            var quantity = order.Pizzas.FirstOrDefault(pizza => pizza.PizzaId == pizzaId)?.Quantity ?? 0;
            if (quantity == 0)
            {
                return;
            }

            this.Dispatcher.Dispatch(new RemovePizzaFromOrderAction
            {
                OrderId = order.Id,
                PizzaId = pizzaId,
                Quantity = quantity - 1,
            });
        }
    }

    private void StartCheckout()
    {
        this.isCheckingOut = true;
    }

    private void CancelCheckout()
    {
        this.isCheckingOut = false;
        this.validationErrorMessage = null;
    }

    private void OnFulfillmentMethodChanged(FulfillmentMethod value)
    {
        this.fulfillmentMethod = value;
        this.validationErrorMessage = null;
    }

    private void SubmitOrder()
    {
        var order = this.OrdersState.Value.Data;
        if (order == null || this.OrdersState.Value.HasPendingCommands)
        {
            return;
        }

        if (this.fulfillmentMethod == FulfillmentMethod.Delivery)
        {
            if (string.IsNullOrWhiteSpace(this.deliveryAddress))
            {
                this.validationErrorMessage = "Delivery address is required.";
                return;
            }

            if (this.deliveryAddress.Length > UpdateOrderRequest.DeliveryAddressMaxLength)
            {
                this.validationErrorMessage =
                    $"Delivery address must be no more than {UpdateOrderRequest.DeliveryAddressMaxLength} characters.";
                return;
            }

            this.validationErrorMessage = null;

            this.Dispatcher.Dispatch(new SubmitDeliveryOrderAction
            {
                OrderId = order.Id,
                DeliveryAddress = this.deliveryAddress,
            });

            return;
        }

        this.validationErrorMessage = null;

        this.Dispatcher.Dispatch(new SubmitPickupOrderAction
        {
            OrderId = order.Id,
        });
    }

    private void ClearError()
    {
        this.Dispatcher.Dispatch(new ClearOrderErrorAction());
    }

    private void RetryOrderRefresh()
    {
        this.Dispatcher.Dispatch(new RefreshOrderAction { OrderId = this.OrderId });
    }

    public static class Selectors
    {
        public const string PizzaMenuHeading = "[role='heading'][aria-level='4']";
        public const string CheckoutButton = $"[role='button'][aria-label='{AriaLabels.Checkout}']";
        public const string SubmitOrderButton = $"[role='button'][aria-label='{AriaLabels.SubmitOrder}']";
        public const string DeliveryAddressInput = $"[aria-label='{AriaLabels.DeliveryAddress}']";
        public const string OrderStatus = "[role='status']";

        public static string AddPizzaButton(string pizzaName) => $"[aria-label='add {pizzaName}']";

        public static string RemovePizzaButton(string pizzaName) => $"[aria-label='remove {pizzaName}']";

        public static string PizzaQuantity(string pizzaName) => $"[aria-label='quantity {pizzaName}']";
    }

    private static class AriaLabels
    {
        public const string Checkout = "checkout";
        public const string SubmitOrder = "submit order";
        public const string DeliveryAddress = "delivery address";
    }
}
