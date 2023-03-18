using Lewee.Blazor.Fluxor;

namespace Sample.Restaurant.Client.States.TableDetails;

public record TableDetailsState : BaseQueryState<TableDetailsDto>
{
    public override string RequestType => "GetTableDetails";
}
