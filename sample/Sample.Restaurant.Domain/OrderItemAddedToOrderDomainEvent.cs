using Lewee.Domain;

namespace Sample.Restaurant.Domain;

public class OrderItemAddedToOrderDomainEvent : IDomainEvent
{
    private readonly decimal price;

    public OrderItemAddedToOrderDomainEvent(
        Guid correlationId,
        Guid orderItemId,
        Guid menuItemId,
        int quantity,
        decimal price,
        Guid orderId,
        Guid tableId)
    {
        this.CorrelationId = correlationId;
        this.OrderItemId = orderItemId;
        this.MenuItemId = menuItemId;
        this.Quantity = quantity;
        this.price = price;
        this.OrderId = orderId;
        this.TableId = tableId;
    }

    public Guid CorrelationId { get; }
    public Guid OrderItemId { get; }
    public Guid MenuItemId { get; }
    public int Quantity { get; }
    public Guid OrderId { get; }
    public Guid TableId { get; }
}
