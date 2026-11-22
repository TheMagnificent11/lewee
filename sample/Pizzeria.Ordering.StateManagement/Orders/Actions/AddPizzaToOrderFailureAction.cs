using Lewee.Infrastructure.Fluxor;

namespace Pizzeria.Ordering.StateManagement.Orders.Actions;

public record AddPizzaToOrderFailureAction : IRequestErrorAction
{
    public Guid CorrelationId { get; init; }

    public string ErrorMessage { get; init; } = string.Empty;
}
