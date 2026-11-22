using Lewee.Infrastructure.Fluxor;

namespace Pizzeria.Ordering.StateManagement.Orders.Actions;

public record StartOrderFailureAction : IRequestErrorAction
{
    public Guid CorrelationId { get; init; }

    public string ErrorMessage { get; init; }
}
