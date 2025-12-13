using Lewee.StateManagement;

namespace Pizzeria.Store.StateManagement.Orders.Actions;

public record AddPizzaToOrderFailureAction : IRequestErrorAction
{
    public Guid CorrelationId { get; init; }

    public string ErrorMessage { get; init; } = string.Empty;
}
