namespace Pizzeria.Ordering.StateManagement.Orders;

public enum OrderCommandType
{
    StartOrder,
    AddPizzaToOrder,
    RemovePizzaFromOrder,
    SubmitPickupOrder,
    SubmitDeliveryOrder,
}
