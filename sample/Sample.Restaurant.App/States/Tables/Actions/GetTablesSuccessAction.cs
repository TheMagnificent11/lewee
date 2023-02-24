using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.App.States.Tables.Actions;

public record GetTablesSuccessAction : IQuerySuccessAction<TableDto[]>
{
    public GetTablesSuccessAction(TableDto[] data)
    {
        this.Data = data;
    }

    public TableDto[] Data { get; }
}
