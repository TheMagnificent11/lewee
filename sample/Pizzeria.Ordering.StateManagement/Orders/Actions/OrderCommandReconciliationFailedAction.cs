namespace Pizzeria.Ordering.StateManagement.Orders.Actions;

public record OrderCommandReconciliationFailedAction
{
    public Guid CorrelationId { get; init; }

    public string ErrorMessage { get; init; } = string.Empty;
}
