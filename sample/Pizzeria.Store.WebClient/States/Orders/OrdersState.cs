namespace Pizzeria.Store.WebClient.States.Orders;

public record OrdersState
{
    public Guid? CurrentOrderId { get; init; }
    public bool IsStartingOrder { get; init; } = false;
    public string? ErrorMessage { get; init; }
    public Dictionary<Guid, int> PizzaQuantities { get; init; } = new();
}
