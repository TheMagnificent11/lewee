using Lewee.Fluxor.Actions;

namespace Sample.Restaurant.App.States.Tables.Actions;

public record GetTables : IRequestAction
{
    public GetTables(Guid correlationId)
    {
        this.CorrelationId = correlationId;
    }

    public Guid CorrelationId { get; }
}
