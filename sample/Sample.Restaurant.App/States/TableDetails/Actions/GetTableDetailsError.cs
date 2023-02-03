using Lewee.Fluxor.Actions;

namespace Sample.Restaurant.App.States.TableDetails.Actions;

public record GetTableDetailsError : IRequestErrorAction
{
    public GetTableDetailsError(string errorMessage)
    {
        this.ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; }
}
