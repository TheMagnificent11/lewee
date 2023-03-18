using Lewee.Blazor.Fluxor;

namespace Sample.Restaurant.Client.States.Tables;

public record TablesState : BaseQueryState<TableDto[]>
{
    public override string RequestType => "GetTables";
}
