using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.StateManagement;
using MediatR;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.StateManagement.Orders.Actions;

namespace Pizzeria.Store.StateManagement.Orders;

public sealed class AddPizzaToOrderEffects :
    RequestEffects<OrdersState, AddPizzaToOrderAction, AddPizzaToOrderSuccessAction, AddPizzaToOrderFailureAction>
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

    protected override async Task ExecuteRequestAsync(
        [NotNull] AddPizzaToOrderAction action,
        [NotNull] IDispatcher dispatcher)
    {
        var result = await this.mediator.Send(new AddPizzaToOrderCommand(
            action.OrderId,
            action.PizzaId,
            action.CorrelationId));

        if (result.IsSuccess)
        {
            dispatcher.Dispatch(new AddPizzaToOrderSuccessAction(action.CorrelationId));
            return;
        }

        dispatcher.Dispatch(new AddPizzaToOrderFailureAction(
            action.CorrelationId,
            result.GenerateErrorMessage()));
    }
}
