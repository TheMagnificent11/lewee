using Lewee.Domain;

namespace Sample.Restaurant.Domain;

public class TableInUseDomainEvent : DomainEvent
{
    public TableInUseDomainEvent(Guid correlationId, Guid tableId, int tableNumber)
    {
        this.CorrelationId = correlationId;
        this.TableId = tableId;
        this.TableNumber = tableNumber;
    }

    public Guid TableId { get; }
    public int TableNumber { get; }
}
