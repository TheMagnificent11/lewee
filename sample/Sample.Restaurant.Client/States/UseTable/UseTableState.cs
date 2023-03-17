using Lewee.Blazor.Fluxor;

namespace Sample.Restaurant.Client.States.UseTable;

public record UseTableState : BaseRequestState
{
    public override string RequestType => "UseTable";

    public int TableNumber { get; init; }
}
