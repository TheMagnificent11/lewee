using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.Client.States.TableDetails.Actions;

public class OrderItemErrorAction : IRequestErrorAction
{
    public OrderItemErrorAction(string errorMessage, Guid correlationId)
    {
        this.ErrorMessage = errorMessage;
        this.CorrelationId = correlationId;
    }

    public string ErrorMessage { get; }
    public Guid CorrelationId { get; }
}
