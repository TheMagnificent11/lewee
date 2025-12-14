using Lewee.StateManagement;
using Pizzeria.Store.Contracts.Orders;

namespace Pizzeria.Store.StateManagement.Orders;

public record OrdersState : CommandState
{
    public OrderDto? CurrentOrder { get; init; }
}
