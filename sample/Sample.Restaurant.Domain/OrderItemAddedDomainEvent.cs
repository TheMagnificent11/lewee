using Lewee.Domain;

namespace Sample.Restaurant.Domain;

public class OrderItemAddedDomainEvent : IDomainEvent
{
    public OrderItemAddedDomainEvent(
        Guid correlationId,
        Guid tableId,
        Guid orderId,
        Guid menuItemId,
        decimal price)
    {
        this.CorrelationId = correlationId;
        this.TableId = tableId;
        this.OrderId = orderId;
        this.MenuItemId = menuItemId;
        this.Price = price;
    }

    public Guid CorrelationId { get; }
    public Guid TableId { get; }
    public Guid OrderId { get; }
    public Guid MenuItemId { get; }
    public decimal Price { get; }
}
