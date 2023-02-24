using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.App.States.TableDetails.Actions;

public record GetTableDetailsAction : IRequestAction
{
    public GetTableDetailsAction(Guid correlationId, int tableNumber)
    {
        this.CorrelationId = correlationId;
        this.TableNumber = tableNumber;
    }

    public Guid CorrelationId { get; }
    public int TableNumber { get; }
}
