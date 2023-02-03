using Lewee.Fluxor;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.App.States.TableDetails;

public record TableDetailsState : BaseQueryState<TableDetailsDto>
{
    public override string RequestType => "GetTableDetails";
}
