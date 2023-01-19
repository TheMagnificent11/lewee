using Lewee.Domain;

namespace Sample.Restaurant.Domain;

public class OrderItem : BaseEntity
{
    private OrderItem(Order order, MenuItem menuItem, int quantity)
    {
        this.OrderId = order.Id;
        this.Order = order;
        this.MenuItem = menuItem;
        this.Quantity = quantity;
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

    public static OrderItem AddToOrder(Order order, MenuItem menuItem, int quantity, Guid correlationId)
    {
        var orderItem = new OrderItem(order, menuItem, quantity);

        orderItem.DomainEvents.Raise(new OrderItemAddedToOrderDomainEvent(
            correlationId,
            orderItem.Id,
            menuItem.Id,
            quantity,
            menuItem.Price,
            order.Id,
            order.TableId));

        return orderItem;
    }
}
