using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.Common;
using Lewee.StateManagement;
using MediatR;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.StateManagement.Orders.Actions;

namespace Pizzeria.Store.StateManagement.Orders;

public sealed class AddPizzaToOrderEffects :
    CommandRequestEffects<OrdersState, AddPizzaToOrderAction, AddPizzaToOrderSuccessAction, AddPizzaToOrderFailureAction, AddPizzaToOrderCompletedAction>
{
    private readonly IMediator mediator;

    public AddPizzaToOrderEffects(
        IState<OrdersState> state,
        IMediator mediator,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<AddPizzaToOrderEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.mediator = mediator;
    }

    protected override async Task<Result> ExecuteRequestAsync(
        [NotNull] AddPizzaToOrderAction action,
        [NotNull] IDispatcher dispatcher)
    {
        return await this.mediator.Send(new AddPizzaToOrderCommand(
            action.OrderId,
            action.PizzaId,
            action.CorrelationId));
    }

    protected override Task ExecuteRequestCompletedAsync(
        [NotNull] AddPizzaToOrderCompletedAction action,
        [NotNull] IDispatcher dispatcher)
    {
        throw new NotSupportedException();
    }
}
