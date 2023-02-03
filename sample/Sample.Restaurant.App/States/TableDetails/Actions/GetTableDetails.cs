using Lewee.Fluxor.Actions;

namespace Sample.Restaurant.App.States.TableDetails.Actions;

public record GetTableDetails : IRequestAction
{
    public GetTableDetails(Guid correlationId)
    {
        this.CorrelationId = correlationId;
    }

    public Guid CorrelationId { get; }
}
