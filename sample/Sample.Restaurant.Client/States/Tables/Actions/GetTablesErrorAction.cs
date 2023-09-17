using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.Client.States.Tables.Actions;

public record GetTablesErrorAction : IRequestErrorAction
{
    public GetTablesErrorAction(string errorMessage, Guid correlationId)
    {
        this.ErrorMessage = errorMessage;
        this.CorrelationId = correlationId;
    }

    public string ErrorMessage { get; }
    public Guid CorrelationId { get; }
}
