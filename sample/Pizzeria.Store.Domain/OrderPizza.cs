using Lewee.Domain;

namespace Pizzeria.Store.Domain;

public class OrderPizza : Relationship
{
    public Guid OrderId { get; internal set; }

    public Order Order { get; internal set; }

    public Guid PizzaId { get; internal set; }

    public Pizza Pizza { get; internal set; }

    public int Quantity { get; internal set; }

    internal static OrderPizza CreateForOrder(Order order, Pizza pizza)
    {
        return new OrderPizza
        {
            OrderId = order.Id,
            PizzaId = pizza.Id,
            Quantity = 1
        };
    }

    internal void IncreaseQuantity()
    {
        this.Quantity++;
    }
}
