using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.Common;
using Lewee.Infrastructure.Fluxor;
using Microsoft.Extensions.Logging;
using Pizzeria.Ordering.StateManagement.Orders.Actions;
using Pizzeria.Store.Contracts.Orders;

namespace Pizzeria.Ordering.StateManagement.Orders;

public sealed class SubmitPickupOrderEffects :
    CommandEffects<OrderState, OrderDto, SubmitPickupOrderAction, SubmitPickupOrderSuccessAction, SubmitPickupOrderFailureAction, SubmitPickupOrderCompletedAction>
{
    private readonly IBffApiClient bffApiClient;
    private readonly IOrderReconciliationCancellationTokenProvider cancellationTokenProvider;

    public SubmitPickupOrderEffects(
        IState<OrderState> state,
        IBffApiClient bffApiClient,
        IOrderReconciliationCancellationTokenProvider cancellationTokenProvider,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<SubmitPickupOrderEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.bffApiClient = bffApiClient;
        this.cancellationTokenProvider = cancellationTokenProvider;
    }

    protected override async Task<CommandResult> ExecuteCommandAsync(
        [NotNull] SubmitPickupOrderAction action,
        [NotNull] IDispatcher dispatcher)
    {
        await this.bffApiClient.UpdateOrderAsync(
            action.OrderId,
            new UpdateOrderRequest { SubmitAsPickup = true },
            this.cancellationTokenProvider.CancellationToken);
        await OrderReconciliation.ReconcileOrderAsync(
            this.bffApiClient,
            action.OrderId,
            action.CorrelationId,
            dispatcher,
            this.Logger,
            this.cancellationTokenProvider.CancellationToken);

        return CommandResult.Success();
    }

    protected override Task ExecuteCommandCompletedAsync(
        [NotNull] SubmitPickupOrderCompletedAction action,
        [NotNull] IDispatcher dispatcher)
    {
        // Order updates are received via SSE
        return Task.CompletedTask;
    }
}
