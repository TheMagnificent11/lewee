using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.Common;
using Lewee.Infrastructure.Fluxor;
using Microsoft.Extensions.Logging;
using Pizzeria.Ordering.StateManagement.Orders.Actions;
using Pizzeria.Store.Contracts.Orders;

namespace Pizzeria.Ordering.StateManagement.Orders;

public sealed class SubmitDeliveryOrderEffects :
    CommandEffects<OrderState, OrderDto, SubmitDeliveryOrderAction, SubmitDeliveryOrderSuccessAction, SubmitDeliveryOrderFailureAction, SubmitDeliveryOrderCompletedAction>
{
    private readonly IBffApiClient bffApiClient;
    private readonly IOrderCommandCorrelationTracker correlationTracker;

    public SubmitDeliveryOrderEffects(
        IState<OrderState> state,
        IBffApiClient bffApiClient,
        IOrderCommandCorrelationTracker correlationTracker,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<SubmitDeliveryOrderEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.bffApiClient = bffApiClient;
        this.correlationTracker = correlationTracker;
    }

    protected override async Task<CommandResult> ExecuteCommandAsync(
        [NotNull] SubmitDeliveryOrderAction action,
        [NotNull] IDispatcher dispatcher)
    {
        this.correlationTracker.Register(action.CorrelationId, OrderCommandType.SubmitDeliveryOrder);

        await this.bffApiClient.UpdateOrderAsync(
            action.OrderId,
            new UpdateOrderRequest { SubmitAsDelivery = true, DeliveryAddress = action.DeliveryAddress });

        return CommandResult.Success();
    }

    protected override Task ExecuteCommandCompletedAsync(
        [NotNull] SubmitDeliveryOrderCompletedAction action,
        [NotNull] IDispatcher dispatcher)
    {
        // Order updates are received via SSE
        return Task.CompletedTask;
    }
}
