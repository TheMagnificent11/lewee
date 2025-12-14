using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.Common;
using Lewee.StateManagement;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.StateManagement.Orders.Actions;

namespace Pizzeria.Store.StateManagement.Orders;

public sealed class StartOrderEffects :
    CommandEffects<OrderState, OrderDto, StartOrderAction, StartOrderSuccessAction, StartOrderFailureAction, StartOrderCompletedAction>
{
    private readonly IMediator mediator;
    private readonly NavigationManager navigationManager;

    public StartOrderEffects(
        IState<OrderState> state,
        IMediator mediator,
        NavigationManager navigationManager,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<StartOrderEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.mediator = mediator;
        this.navigationManager = navigationManager;
    }

    protected override async Task<Result> ExecuteRequestAsync(
        [NotNull] StartOrderAction action,
        [NotNull] IDispatcher dispatcher)
    {
        return await this.mediator.Send(new StartOrderCommand(action.CorrelationId));
    }

    protected override Task ExecuteRequestCompletedAsync(
        [NotNull] StartOrderCompletedAction action,
        [NotNull] IDispatcher dispatcher)
    {
        if (action.Data.Id == Guid.Empty)
        {
            this.Logger.LogWarning("Order ID is null or empty. Navigation to order details page aborted.");
            return Task.CompletedTask;
        }

        this.navigationManager.NavigateTo(PageRoutes.GetOrderRoute(action.Data.Id));

        return Task.CompletedTask;
    }
}
