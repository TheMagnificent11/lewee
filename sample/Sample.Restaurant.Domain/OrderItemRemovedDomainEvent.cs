using Lewee.Domain;
using Sample.Restaurant.Contracts.ClientMessages;

namespace Sample.Restaurant.Domain;

public class OrderItemRemovedDomainEvent : DomainEvent, IToClientEvent
{
    public OrderItemRemovedDomainEvent(
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

    public ClientEvent ToClientEvent(Guid correlationId, string? userId)
    {
        var message = new ItemRemovedMessage
        {
            TableNumber = this.TableNumber
        };

        return new ClientEvent(correlationId, userId, message);
    }
}
