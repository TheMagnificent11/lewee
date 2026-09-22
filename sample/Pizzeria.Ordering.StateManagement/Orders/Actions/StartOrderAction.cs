using Lewee.Infrastructure.Fluxor;

namespace Pizzeria.Ordering.StateManagement.Orders.Actions;

public record StartOrderAction : IRequestAction
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
