using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.Client.States.TableDetails.Actions;

public record RemoveItemCompletedAction : IMessageReceivedAction
{
    public RemoveItemCompletedAction(Guid correlationId, int tableNumber)
    {
        this.CorrelationId = correlationId;
        this.TableNumber = tableNumber;
    }

    public Guid CorrelationId { get; }
    public int TableNumber { get; }
    public string RequestType => "RemoveItem";
}
