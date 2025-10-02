using Lewee.Blazor.Fluxor;
using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.WebClient.States.Orders;

public record OrdersState : RequestState
{
    public OrderDto? CurrentOrder { get; init; }
    public bool IsStartingOrder { get; init; } = false;
}
