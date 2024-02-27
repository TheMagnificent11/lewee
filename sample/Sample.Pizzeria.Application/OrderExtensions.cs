using Sample.Pizzeria.Contracts;
using Sample.Pizzeria.Domain;

namespace Sample.Pizzeria.Application;

internal static class OrderExtensions
{
    public static OrderDto ToDto(this Order order)
    {
        var pizzas = order.Pizzas
            .Select(x => x.ToDto())
            .ToArray();

        var orderDate = order.StatusId == OrderStatus.New
            ? order.CreatedAtUtc
            : order.OrderPlacedDateTime;

        return new OrderDto(order.Id, orderDate, order.Status.Name, pizzas);
    }
}
