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

        return new OrderDto(order.Id, order.Status.Name, pizzas);
    }
}
