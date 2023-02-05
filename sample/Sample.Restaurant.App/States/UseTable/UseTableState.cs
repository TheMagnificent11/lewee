using Lewee.Fluxor;

namespace Sample.Restaurant.App.States.UseTable;

public record UseTableState : BaseRequestState
{
    public override string RequestType => "UseTable";

    public int TableNumber { get; init; }
}
