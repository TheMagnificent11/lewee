using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.Client.States.TableDetails.Actions;

public record RemoveItemErrorAction : IRequestErrorAction
{
    public RemoveItemErrorAction(string errorMessage)
    {
        this.ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; }

    public string RequestType => "RemoveItem";
}
