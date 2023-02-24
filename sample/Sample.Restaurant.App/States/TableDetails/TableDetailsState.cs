using Lewee.Blazor.Fluxor;

namespace Sample.Restaurant.App.States.TableDetails;

public record TableDetailsState : BaseQueryState<TableDetailsDto>
{
    public override string RequestType => "GetTableDetails";
}
