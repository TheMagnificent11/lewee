using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.Client.States.TableDetails.Actions;

public record RemoveItemErrorAction : IRequestErrorAction
{
    public RemoveItemErrorAction(string errorMessage, Guid correlationId)
    {
        this.ErrorMessage = errorMessage;
        this.CorrelationId = correlationId;
    }

    public string ErrorMessage { get; }
    public Guid CorrelationId { get; }
}
