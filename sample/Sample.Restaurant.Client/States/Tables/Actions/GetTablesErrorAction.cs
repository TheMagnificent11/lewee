using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.Client.States.Tables.Actions;

public record GetTablesErrorAction : IRequestErrorAction
{
    public GetTablesErrorAction(string errorMessage)
    {
        this.ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; }
}
