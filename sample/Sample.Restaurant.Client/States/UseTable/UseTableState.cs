using Lewee.Blazor.Fluxor;

namespace Sample.Restaurant.Client.States.UseTable;

public record UseTableState : RequestState
{
    public override string RequestType => "UseTable";

    public int TableNumber { get; init; }
}
