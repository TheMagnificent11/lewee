using Lewee.Infrastructure.Fluxor;

namespace Pizzeria.Ordering.StateManagement.Orders.Actions;

public record SubmitDeliveryOrderFailureAction : IRequestErrorAction
{
    public Guid CorrelationId { get; init; }

    public string ErrorMessage { get; init; } = string.Empty;
}
