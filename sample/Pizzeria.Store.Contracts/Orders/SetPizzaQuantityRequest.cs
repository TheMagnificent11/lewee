namespace Pizzeria.Store.Contracts.Orders;

public sealed record SetPizzaQuantityRequest
{
    public int Quantity { get; init; }
}
