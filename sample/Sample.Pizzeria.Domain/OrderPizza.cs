using Lewee.Domain;

namespace Sample.Pizzeria.Domain;

public class OrderPizza : Entity
{
    internal OrderPizza(Order order, int pizzaId)
    {
        this.OrderId = order.Id;
        this.Order = order;
        this.PizzaId = pizzaId;
        this.Quantity = 1;
    }

    private OrderPizza()
    {
        // Required for EF Core
    }

    public Guid OrderId { get; protected set; }
    public Order Order { get; protected set; }
    public int PizzaId { get; protected set; }
    public int Quantity { get; protected set; }

    internal void IncreaseQuantity(int quantity)
    {
        this.Quantity += quantity;
        this.IsDeleted = false;
    }

    internal void DecreaseQuantity(int quantity)
    {
        this.Quantity -= quantity;

        if (this.Quantity <= 0)
        {
            this.IsDeleted = true;
        }
    }
}
