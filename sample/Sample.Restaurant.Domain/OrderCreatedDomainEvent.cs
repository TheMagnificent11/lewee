using Lewee.Domain;

namespace Sample.Restaurant.Domain;

public class OrderCreatedDomainEvent : IDomainEvent
{
    public OrderCreatedDomainEvent(
        Guid correlationId,
        Guid orderId,
        Guid tableId,
        int tableNumber,
        DateTime createdDateTimeUtc)
    {
        this.CorrelationId = correlationId;
        this.OrderId = orderId;
        this.TableId = tableId;
        this.TableNumber = tableNumber;
        this.CreatedDateTimeUtc = createdDateTimeUtc;
    }

    public Guid CorrelationId { get; }
    public Guid OrderId { get; }
    public Guid TableId { get; }
    public int TableNumber { get; }
    public DateTime CreatedDateTimeUtc { get; }
}
