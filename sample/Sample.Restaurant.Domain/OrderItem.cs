using Lewee.Domain;

namespace Sample.Restaurant.Domain;

public class OrderItem : Entity
{
    private OrderItem(Order order, MenuItem menuItem)
        : base(Guid.Empty)
    {
        this.OrderId = order.Id;
        this.Order = order;
        this.MenuItemId = menuItem.Id;
        this.MenuItem = menuItem;
        this.Quantity = 1;
        this.Price = menuItem.Price;
    }

    // EF Core constructor
    private OrderItem()
    {
        this.Order = Order.EmptyOrder;
        this.MenuItem = MenuItem.EmptyMenuItem;
    }

    public Guid OrderId { get; protected set; }
    public Order Order { get; protected set; }
    public Guid MenuItemId { get; protected set; }
    public MenuItem MenuItem { get; protected set; }
    public int Quantity { get; protected set; }
    public decimal Price { get; protected set; }
    public decimal ItemTotal => this.Quantity * this.Price;

    internal static OrderItem AddToOrder(Order order, MenuItem menuItem)
    {
        return new OrderItem(order, menuItem);
    }

    internal void IncreaseQuantity()
    {
        this.Quantity++;
    }

    internal void ReduceQuantity()
    {
        if (this.Quantity == 0)
        {
            return;
        }

        this.Quantity--;
    }
}
