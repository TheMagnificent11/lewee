using Lewee.Domain;
using Sample.Restaurant.Contracts.ClientMessages;

namespace Sample.Restaurant.Domain;

public class TableInUseDomainEvent : DomainEvent, IToClientEvent
{
    public TableInUseDomainEvent(Guid correlationId, Guid tableId, int tableNumber)
    {
        this.CorrelationId = correlationId;
        this.TableId = tableId;
        this.TableNumber = tableNumber;
    }

    public Guid TableId { get; }
    public int TableNumber { get; }

    public ClientEvent ToClientEvent(Guid correlationId, string? userId)
    {
        var message = new TableUsedMessage
        {
            TableNumber = this.TableNumber
        };

        return new ClientEvent(correlationId, userId, message);
    }
}
