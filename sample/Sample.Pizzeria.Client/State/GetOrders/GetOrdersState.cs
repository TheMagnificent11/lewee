using Lewee.Blazor.Fluxor;
using Sample.Pizzeria.Contracts;

namespace Sample.Pizzeria.Client.State.GetOrders;

public record GetOrdersState : QueryState<OrderDto[]>
{
}
