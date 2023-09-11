using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.Client.States.UseTable.Actions;

public record UseTableSuccessAction : IRequestSuccessAction
{
    public string RequestType => "UseTable";
}
