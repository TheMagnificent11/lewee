using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.Client.States.UseTable.Actions;
public record UseTableAction : IRequestAction
{
    public UseTableAction(Guid correlationId, int tableNumber)
    {
        this.CorrelationId = correlationId;
        this.TableNumber = tableNumber;
    }

    public Guid CorrelationId { get; }
    public int TableNumber { get; }

    public string RequestType => "UseTable";
}
