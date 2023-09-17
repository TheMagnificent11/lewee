using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.Client.States.UseTable.Actions;

public class UseTableCompletedAction : IMessageReceivedAction
{
    public UseTableCompletedAction(Guid correlationId, int tableNumber)
    {
        this.CorrelationId = correlationId;
        this.TableNumber = tableNumber;
    }

    public Guid CorrelationId { get; }
    public int TableNumber { get; }
    public string RequestType => "UseTable";
}
