namespace Pizzeria.Ordering.StateManagement.Orders;

public interface IOrderReconciliationCancellationTokenProvider
{
    CancellationToken CancellationToken { get; }
}
