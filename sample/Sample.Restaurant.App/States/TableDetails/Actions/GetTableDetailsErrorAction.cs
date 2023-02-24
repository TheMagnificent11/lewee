using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.App.States.TableDetails.Actions;

public record GetTableDetailsErrorAction : IRequestErrorAction
{
    public GetTableDetailsErrorAction(string errorMessage)
    {
        this.ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; }
}
