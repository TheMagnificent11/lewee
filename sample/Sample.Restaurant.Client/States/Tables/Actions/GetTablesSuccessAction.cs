using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.Client.States.Tables.Actions;

public record GetTablesSuccessAction : IQuerySuccessAction<TableDto[]>
{
    public GetTablesSuccessAction(TableDto[] data, Guid correlationId)
    {
        this.Data = data;
        this.CorrelationId = correlationId;
    }

    public TableDto[] Data { get; }
    public Guid CorrelationId { get; }
}
