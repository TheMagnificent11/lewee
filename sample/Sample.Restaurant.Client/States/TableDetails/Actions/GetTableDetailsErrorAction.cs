using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.Client.States.TableDetails.Actions;

public record GetTableDetailsErrorAction : IRequestErrorAction
{
    public GetTableDetailsErrorAction(string errorMessage, Guid correlationId)
    {
        this.ErrorMessage = errorMessage;
        this.CorrelationId = correlationId;
    }

    public string ErrorMessage { get; }
    public Guid CorrelationId { get; }
}
