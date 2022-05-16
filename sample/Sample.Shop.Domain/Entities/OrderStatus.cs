namespace Sample.Shop.Domain.Entities;

public enum OrderStatus
{
    ShoppingCart = 0,
    Checkout,
    CustomerDetailsAdded,
    PaymentProcessing,
    PaymentComplete,
    OrderPlaced,
    ShippingStarted,
    ShippingComplete
}
