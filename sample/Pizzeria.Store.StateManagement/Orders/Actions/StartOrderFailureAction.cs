using Lewee.StateManagement;

namespace Pizzeria.Store.StateManagement.Orders.Actions;

public record StartOrderFailureAction : IRequestErrorAction
{
    public StartOrderFailureAction(Guid correlationId, string errorMessage)
    {
        this.CorrelationId = correlationId;
        this.ErrorMessage = errorMessage;
    }

    public Guid CorrelationId { get; init; }
    public string ErrorMessage { get; init; }
}
