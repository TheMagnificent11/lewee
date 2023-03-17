using Lewee.Domain;

namespace Sample.Restaurant.Domain;

public class OrderItemAddedDomainEvent : BaseDomainEvent
{
    public OrderItemAddedDomainEvent(
        Guid correlationId,
        Guid tableId,
        int tableNumber,
        Guid orderId,
        Guid menuItemId,
        decimal price)
    {
        this.CorrelationId = correlationId;
        this.TableId = tableId;
        this.TableNumber = tableNumber;
        this.OrderId = orderId;
        this.MenuItemId = menuItemId;
        this.Price = price;
    }

    public Guid TableId { get; }
    public int TableNumber { get; }
    public Guid OrderId { get; }
    public Guid MenuItemId { get; }
    public decimal Price { get; }
}
