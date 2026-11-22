using System.Diagnostics.CodeAnalysis;
using Fluxor;
using Microsoft.Extensions.Logging;
using Pizzeria.Ordering.StateManagement.Orders.Actions;

namespace Pizzeria.Ordering.StateManagement.Orders;

public sealed class RefreshOrderEffects
{
    private readonly IBffApiClient bffApiClient;
    private readonly IOrderReconciliationCancellationTokenProvider cancellationTokenProvider;
    private readonly ILogger<RefreshOrderEffects> logger;

    public RefreshOrderEffects(
        IBffApiClient bffApiClient,
        IOrderReconciliationCancellationTokenProvider cancellationTokenProvider,
        ILogger<RefreshOrderEffects> logger)
    {
        this.bffApiClient = bffApiClient;
        this.cancellationTokenProvider = cancellationTokenProvider;
        this.logger = logger;
    }

    [EffectMethod]
    public async Task RefreshOrderAsync([NotNull] RefreshOrderAction action, [NotNull] IDispatcher dispatcher)
    {
        try
        {
            var order = await this.bffApiClient.GetOrderAsync(
                action.OrderId,
                this.cancellationTokenProvider.CancellationToken);

            dispatcher.Dispatch(new OrderUpdatedAction { Data = order });
        }
        catch (OperationCanceledException exception)
            when (this.cancellationTokenProvider.CancellationToken.IsCancellationRequested)
        {
            this.logger.LogDebug(exception, "Order refresh was canceled for {OrderId}", action.OrderId);
        }
        catch (Exception exception)
        {
            this.logger.LogError(exception, "Failed to refresh order {OrderId}", action.OrderId);
        }
    }
}
