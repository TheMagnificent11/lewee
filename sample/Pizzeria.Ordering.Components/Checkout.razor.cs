using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Pizzeria.Ordering.StateManagement.Orders;
using Pizzeria.Ordering.StateManagement.Orders.Actions;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.Contracts.Orders;

namespace Pizzeria.Ordering.Components;

public partial class Checkout : FluxorComponent
{
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

        if (this.OrdersState.Value.Data.Status != OrderStatus.InProgress)
        {
            this.Navigation.NavigateTo(PageRoutes.GetOrderRoute(this.OrdersState.Value.Data.Id));
        }
    }

    private void OnFulfillmentMethodChanged(FulfillmentMethod value)
    {
        this.fulfillmentMethod = value;
        this.validationErrorMessage = null;
    }

    private void SubmitOrder()
    {
        var order = this.OrdersState.Value.Data;
        if (order == null)
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

    public static class Selectors
    {
        public const string CheckoutHeading = "[role='heading'][aria-level='4']";
        public const string SubmitOrderButton = $"[role='button'][aria-label='{AriaLabels.SubmitOrder}']";
        public const string DeliveryAddressInput = $"[aria-label='{AriaLabels.DeliveryAddress}']";
    }

    private static class AriaLabels
    {
        public const string SubmitOrder = "submit order";
        public const string DeliveryAddress = "delivery address";
    }
}
