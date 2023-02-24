using Lewee.Blazor.Fluxor;

namespace Sample.Restaurant.App.States.Tables;

public record TablesState : BaseQueryState<TableDto[]>
{
    public override string RequestType => "GetTables";
}
