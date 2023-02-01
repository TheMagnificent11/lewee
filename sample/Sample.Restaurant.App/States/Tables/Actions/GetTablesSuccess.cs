using Lewee.Fluxor.Actions;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.App.States.Tables.Actions;

public record GetTablesSuccess : IQuerySuccessAction<TableDto[]>
{
    public GetTablesSuccess(TableDto[] data)
    {
        this.Data = data;
    }

    public TableDto[] Data { get; }
}
