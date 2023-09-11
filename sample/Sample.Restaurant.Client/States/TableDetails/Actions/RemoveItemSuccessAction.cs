using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.Client.States.TableDetails.Actions;

public record RemoveItemSuccessAction : IRequestSuccessAction
{
    public string RequestType => "RemoveItem";
}
