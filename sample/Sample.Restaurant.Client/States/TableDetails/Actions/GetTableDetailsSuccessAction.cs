using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.Client.States.TableDetails.Actions;

public record GetTableDetailsSuccessAction : IQuerySuccessAction<TableDetailsDto>
{
    public GetTableDetailsSuccessAction(TableDetailsDto data, Guid correlationId)
    {
        this.CorrelationId = correlationId;
        this.Data = data;
    }

    public Guid CorrelationId { get; }
    public TableDetailsDto Data { get; }
}
