using Lewee.Fluxor.Actions;

namespace Sample.Restaurant.App.States.Tables.Actions;

public record GetTablesError : IRequestErrorAction
{
    public GetTablesError(string errorMessage)
    {
        this.ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; }
}
