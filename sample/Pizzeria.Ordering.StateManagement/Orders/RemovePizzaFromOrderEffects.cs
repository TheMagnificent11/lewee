using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.Common;
using Lewee.Infrastructure.Fluxor;
using Microsoft.Extensions.Logging;
using Pizzeria.Ordering.StateManagement.Orders.Actions;
using Pizzeria.Store.Contracts.Orders;

namespace Pizzeria.Ordering.StateManagement.Orders;

public sealed class RemovePizzaFromOrderEffects :
    CommandEffects<OrderState, OrderDto, RemovePizzaFromOrderAction, RemovePizzaFromOrderSuccessAction, RemovePizzaFromOrderFailureAction, RemovePizzaFromOrderCompletedAction>
{
    private readonly IBffApiClient bffApiClient;
    private readonly IOrderReconciliationCancellationTokenProvider cancellationTokenProvider;

    public RemovePizzaFromOrderEffects(
        IState<OrderState> state,
        IBffApiClient bffApiClient,
        IOrderReconciliationCancellationTokenProvider cancellationTokenProvider,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<RemovePizzaFromOrderEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.bffApiClient = bffApiClient;
        this.cancellationTokenProvider = cancellationTokenProvider;
    }

    protected override async Task<CommandResult> ExecuteCommandAsync(
        [NotNull] RemovePizzaFromOrderAction action,
        [NotNull] IDispatcher dispatcher)
    {
        if (action.Quantity == 0)
        {
            await this.bffApiClient.RemovePizzaFromOrderAsync(
                action.OrderId,
                action.PizzaId,
                this.cancellationTokenProvider.CancellationToken);
        }
        else
        {
            await this.bffApiClient.SetOrderPizzaQuantityAsync(
                action.OrderId,
                action.PizzaId,
                new SetPizzaQuantityRequest { Quantity = action.Quantity },
                this.cancellationTokenProvider.CancellationToken);
        }

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
        [NotNull] RemovePizzaFromOrderCompletedAction action,
        [NotNull] IDispatcher dispatcher)
    {
        // Order updates are received via SSE
        return Task.CompletedTask;
    }
}
