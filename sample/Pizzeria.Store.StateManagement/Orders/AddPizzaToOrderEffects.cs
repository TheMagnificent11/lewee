using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.Common;
using Lewee.Infrastructure.Fluxor;
using MediatR;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.StateManagement.Orders.Actions;

namespace Pizzeria.Store.StateManagement.Orders;

public sealed class AddPizzaToOrderEffects :
    CommandEffects<OrderState, OrderDto, AddPizzaToOrderAction, AddPizzaToOrderSuccessAction, AddPizzaToOrderFailureAction, AddPizzaToOrderCompletedAction>
{
    private readonly IMediator mediator;

    public AddPizzaToOrderEffects(
        IState<OrderState> state,
        IMediator mediator,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<AddPizzaToOrderEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.mediator = mediator;
    }

    protected override async Task<CommandResult> ExecuteCommandAsync(
        [NotNull] AddPizzaToOrderAction action,
        [NotNull] IDispatcher dispatcher)
    {
        return await this.mediator.Send(new AddPizzaToOrderCommand(
            action.OrderId,
            action.PizzaId,
            action.CorrelationId));
    }

    protected override Task ExecuteCommandCompletedAsync(
        [NotNull] AddPizzaToOrderCompletedAction action,
        [NotNull] IDispatcher dispatcher)
    {
        // Order updates are received via SSE
        return Task.CompletedTask;
    }
}
