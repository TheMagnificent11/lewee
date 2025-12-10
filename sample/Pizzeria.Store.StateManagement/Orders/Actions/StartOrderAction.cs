using Lewee.StateManagement;

namespace Pizzeria.Store.StateManagement.Orders.Actions;

public record StartOrderAction : IRequestAction
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
