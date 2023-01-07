using Lewee.Domain;

namespace Sample.Restaurant.Domain;

public class TableInUseDomainEvent : IDomainEvent
{
    public TableInUseDomainEvent(Guid correlationId, Guid tableId, int tableNumber)
    {
        this.CorrelationId = correlationId;
        this.TableId = tableId;
        this.TableNumber = tableNumber;
    }

    public Guid CorrelationId { get; }
    public Guid TableId { get; }
    public int TableNumber { get; }
}
