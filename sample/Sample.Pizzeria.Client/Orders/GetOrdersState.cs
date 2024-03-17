using Lewee.Blazor.Fluxor;
using Sample.Pizzeria.Contracts;

namespace Sample.Pizzeria.Client.Orders;

public record GetOrdersState : QueryState<OrderDto[]>
{
}
