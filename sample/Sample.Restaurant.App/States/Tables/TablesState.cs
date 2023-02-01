using Lewee.Fluxor;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.App.States.Tables;

public record TablesState : BaseQueryState<TableDto[]>
{
    public override string RequestType => "GetTables";
}
