using Sample.Pizzeria.Contracts;
using Sample.Pizzeria.Domain;

namespace Sample.Pizzeria.Application;

internal static class OrderPizzaExtensions
{
    public static OrderPizzaDto ToDto(this OrderPizza orderPizza)
    {
        var pizza = Menu.Pizzas.FirstOrDefault(x => x.Id == orderPizza.PizzaId)
            ?? throw new InvalidOperationException($"Pizza with id {orderPizza.PizzaId} not found");

        return new OrderPizzaDto(orderPizza.Id, pizza.Name, pizza.Price, orderPizza.Quantity);
    }
}
