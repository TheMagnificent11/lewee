using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.Common;
using Lewee.Infrastructure.Fluxor;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.StateManagement.Orders.Actions;

namespace Pizzeria.Store.StateManagement.Orders;

public sealed class AddPizzaToOrderEffects :
    CommandEffects<OrderState, OrderDto, AddPizzaToOrderAction, AddPizzaToOrderSuccessAction, AddPizzaToOrderFailureAction, AddPizzaToOrderCompletedAction>
{
    private readonly IBffApiClient bffApiClient;

    public AddPizzaToOrderEffects(
        IState<OrderState> state,
        IBffApiClient bffApiClient,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<AddPizzaToOrderEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.bffApiClient = bffApiClient;
    }

    protected override async Task<CommandResult> ExecuteCommandAsync(
        [NotNull] AddPizzaToOrderAction action,
        [NotNull] IDispatcher dispatcher)
    {
        await this.bffApiClient.AddPizzaToOrderAsync(action.OrderId, action.PizzaId);

        return CommandResult.Success();
    }

    protected override Task ExecuteCommandCompletedAsync(
        [NotNull] AddPizzaToOrderCompletedAction action,
        [NotNull] IDispatcher dispatcher)
    {
        // Order updates are received via SSE
        return Task.CompletedTask;
    }
}
