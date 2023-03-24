using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.Client.States.UseTable.Actions;

public record UseTableErrorAction : IRequestErrorAction
{
    public UseTableErrorAction(string errorMessage)
    {
        this.ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; }
}
