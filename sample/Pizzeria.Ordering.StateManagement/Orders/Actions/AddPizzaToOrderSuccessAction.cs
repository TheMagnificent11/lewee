using Lewee.Infrastructure.Fluxor;

namespace Pizzeria.Ordering.StateManagement.Orders.Actions;

public record AddPizzaToOrderSuccessAction : IRequestSuccessAction
{
    public Guid CorrelationId { get; init; }
}
