using Lewee.Contracts.StateManagement;

namespace Pizzeria.Store.Contracts.Orders.Actions;

public record StartOrderAction : IRequestAction
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
