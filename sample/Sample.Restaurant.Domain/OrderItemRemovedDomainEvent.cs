using Lewee.Domain;

namespace Sample.Restaurant.Domain;

public class OrderItemRemovedDomainEvent : IDomainEvent
{
    public OrderItemRemovedDomainEvent(
        Guid correlationId,
        Guid orderItemId,
        Guid menuItemId,
        decimal price,
        Guid orderId,
        Guid tableId)
    {
        this.CorrelationId = correlationId;
        this.OrderItemId = orderItemId;
        this.MenuItemId = menuItemId;
        this.Price = price;
        this.OrderId = orderId;
        this.TableId = tableId;
    }

    public Guid CorrelationId { get; }
    public Guid OrderItemId { get; }
    public Guid MenuItemId { get; }
    public decimal Price { get; }
    public Guid OrderId { get; }
    public Guid TableId { get; }
}
