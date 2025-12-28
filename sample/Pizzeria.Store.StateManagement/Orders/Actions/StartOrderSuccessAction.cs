using Lewee.Infrastructure.Fluxor;

namespace Pizzeria.Store.StateManagement.Orders.Actions;

public record StartOrderSuccessAction : IRequestSuccessAction
{
    public Guid CorrelationId { get; init; }
}
