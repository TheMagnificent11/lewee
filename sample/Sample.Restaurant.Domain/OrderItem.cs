using Lewee.Domain;

namespace Sample.Restaurant.Domain;

public class OrderItem : BaseEntity
{
    private OrderItem(Order order, MenuItem menuItem)
    {
        this.OrderId = order.Id;
        this.Order = order;
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

    public static OrderItem AddToOrder(Order order, MenuItem menuItem, Guid correlationId)
    {
        var orderItem = new OrderItem(order, menuItem);

        orderItem.DomainEvents.Raise(new OrderItemAddedDomainEvent(
            correlationId,
            orderItem.Id,
            menuItem.Id,
            menuItem.Price,
            order.Id,
            order.TableId));

        return orderItem;
    }

    public void IncreaseQuantity(Guid correlationId)
    {
        this.Quantity++;

        this.DomainEvents.Raise(new OrderItemAddedDomainEvent(
            correlationId,
            this.Id,
            this.MenuItemId,
            this.MenuItem.Price,
            this.OrderId,
            this.Order.TableId));
    }

    public void ReduceQuantity(Guid correlationId)
    {
        if (this.Quantity == 0)
        {
            return;
        }

        this.Quantity--;

        this.DomainEvents.Raise(new OrderItemRemovedDomainEvent(
            correlationId,
            this.Id,
            this.MenuItemId,
            this.MenuItem.Price,
            this.OrderId,
            this.Order.TableId));
    }
}
