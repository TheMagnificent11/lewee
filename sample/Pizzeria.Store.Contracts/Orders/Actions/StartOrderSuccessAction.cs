using Lewee.Contracts.StateManagement;

namespace Pizzeria.Store.Contracts.Orders.Actions;

public record StartOrderSuccessAction : IRequestSuccessAction
{
    public Guid CorrelationId { get; init; }
}
