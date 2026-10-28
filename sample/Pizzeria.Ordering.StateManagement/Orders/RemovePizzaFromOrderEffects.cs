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
    private readonly IOrderCommandCorrelationTracker correlationTracker;

    public RemovePizzaFromOrderEffects(
        IState<OrderState> state,
        IBffApiClient bffApiClient,
        IOrderCommandCorrelationTracker correlationTracker,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<RemovePizzaFromOrderEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.bffApiClient = bffApiClient;
        this.correlationTracker = correlationTracker;
    }

    protected override async Task<CommandResult> ExecuteCommandAsync(
        [NotNull] RemovePizzaFromOrderAction action,
        [NotNull] IDispatcher dispatcher)
    {
        this.correlationTracker.Register(action.CorrelationId, OrderCommandType.RemovePizzaFromOrder);

        await this.bffApiClient.UpdateOrderAsync(
            action.OrderId,
            new UpdateOrderRequest { RemovePizzaId = action.PizzaId });

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
