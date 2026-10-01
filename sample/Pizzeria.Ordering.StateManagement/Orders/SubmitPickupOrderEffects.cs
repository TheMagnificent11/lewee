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

    public SubmitPickupOrderEffects(
        IState<OrderState> state,
        IBffApiClient bffApiClient,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<SubmitPickupOrderEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.bffApiClient = bffApiClient;
    }

    protected override async Task<CommandResult> ExecuteCommandAsync(
        [NotNull] SubmitPickupOrderAction action,
        [NotNull] IDispatcher dispatcher)
    {
        await this.bffApiClient.UpdateOrderAsync(
            action.OrderId,
            new UpdateOrderRequest { SubmitAsPickup = true });

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
