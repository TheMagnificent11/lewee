using Lewee.Infrastructure.Fluxor;

namespace Pizzeria.Ordering.StateManagement.Orders.Actions;

public record SubmitDeliveryOrderAction : IRequestAction
{
    public Guid OrderId { get; init; }

    public string DeliveryAddress { get; init; } = string.Empty;

    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
