using Lewee.Contracts.StateManagement;

namespace Pizzeria.Store.Contracts.Orders.Actions;

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
