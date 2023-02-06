using Lewee.Fluxor.Actions;

namespace Sample.Restaurant.App.States.Tables.Actions;

public record GetTablesErrorAction : IRequestErrorAction
{
    public GetTablesErrorAction(string errorMessage)
    {
        this.ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; }
}
