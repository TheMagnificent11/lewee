using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.Client.States.Tables.Actions;

public record GetTablesAction : IRequestAction
{
    public GetTablesAction(Guid correlationId)
    {
        this.CorrelationId = correlationId;
    }

    public Guid CorrelationId { get; }
}
