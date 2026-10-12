using Lewee.Infrastructure.Fluxor;

namespace Pizzeria.Ordering.StateManagement.Orders.Actions;

public record SubmitPickupOrderSuccessAction : IRequestSuccessAction
{
    public Guid CorrelationId { get; init; }
}
