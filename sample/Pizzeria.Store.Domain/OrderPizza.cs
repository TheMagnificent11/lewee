using Lewee.Domain;

namespace Pizzeria.Store.Domain;

public class OrderPizza : Relationship
{
    public Guid OrderId { get; internal set; }

    public Order Order { get; internal set; }

    public Guid PizzaId { get; internal set; }

    public Pizza Pizza { get; internal set; }

    public string PizzaName { get; internal set; }

    public decimal PizzaPrice { get; internal set; }

    public int Quantity { get; internal set; }

    internal static OrderPizza CreateForOrder(Order order, Pizza pizza, int quantity = 1)
    {
        return new OrderPizza
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            PizzaId = pizza.Id,
            Pizza = pizza,
            PizzaName = pizza.Name,
            PizzaPrice = pizza.Price,
            Quantity = quantity,
        };
    }

    internal void IncreaseQuantity()
    {
        this.Quantity++;
    }

    internal void DecreaseQuantity()
    {
        this.Quantity--;
    }

    internal void SetQuantity(int quantity)
    {
        this.Quantity = quantity;
    }
}
