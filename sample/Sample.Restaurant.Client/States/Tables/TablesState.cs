using Lewee.Blazor.Fluxor;

namespace Sample.Restaurant.Client.States.Tables;

public record TablesState : QueryState<TableDto[]>
{
    public override string RequestType => "GetTables";
}
