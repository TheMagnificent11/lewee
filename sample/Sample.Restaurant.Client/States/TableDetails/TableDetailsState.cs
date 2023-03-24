using Lewee.Blazor.Fluxor;

namespace Sample.Restaurant.Client.States.TableDetails;

public record TableDetailsState : QueryState<TableDetailsDto>
{
    public override string RequestType => "GetTableDetails";

    public bool IsSaving { get; set; }
}
