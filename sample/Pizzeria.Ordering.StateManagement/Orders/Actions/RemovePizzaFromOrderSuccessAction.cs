using Lewee.Infrastructure.Fluxor;

namespace Pizzeria.Ordering.StateManagement.Orders.Actions;

public record RemovePizzaFromOrderSuccessAction : IRequestSuccessAction
{
    public Guid CorrelationId { get; init; }
}
