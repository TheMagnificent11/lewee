using Lewee.Blazor.Fluxor;

namespace Pizzeria.Store.WebClient.States.Orders;

public record OrdersState : RequestState
{
    public Guid? CurrentOrderId { get; init; }
    public bool IsStartingOrder { get; init; } = false;
    public Dictionary<Guid, int> PizzaQuantities { get; init; } = new();
}
