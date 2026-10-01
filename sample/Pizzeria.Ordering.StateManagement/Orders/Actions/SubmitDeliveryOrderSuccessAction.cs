using Lewee.Infrastructure.Fluxor;

namespace Pizzeria.Ordering.StateManagement.Orders.Actions;

public record SubmitDeliveryOrderSuccessAction : IRequestSuccessAction
{
    public Guid CorrelationId { get; init; }
}
