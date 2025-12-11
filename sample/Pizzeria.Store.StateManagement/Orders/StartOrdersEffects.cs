using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.StateManagement;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.StateManagement.Orders.Actions;

namespace Pizzeria.Store.StateManagement.Orders;

public sealed class StartOrdersEffects :
    RequestEffects<OrdersState, StartOrderAction, StartOrderSuccessAction, StartOrderFailureAction>
{
    private readonly IMediator mediator;
    private readonly NavigationManager navigationManager;

    public StartOrdersEffects(
        IState<OrdersState> state,
        IMediator mediator,
        NavigationManager navigationManager,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<StartOrdersEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.mediator = mediator;
        this.navigationManager = navigationManager;
    }

    [EffectMethod]
    public Task OnStartOrderCompletedAsync(
        [NotNull] StartOrderCompletedAction action,
        [NotNull] IDispatcher _)
    {
        if (action.Order is null || action.Order.Id == Guid.Empty)
        {
            this.Logger.LogWarning("Order ID is null or empty. Navigation to order details page aborted.");
            return Task.CompletedTask;
        }

        this.navigationManager.NavigateTo(PageRoutes.GetOrderRoute(action.Order.Id));

        return Task.CompletedTask;
    }

    protected override async Task ExecuteRequestAsync(
        [NotNull] StartOrderAction action,
        [NotNull] IDispatcher dispatcher)
    {
        try
        {
            var result = await this.mediator.Send(new StartOrderCommand(action.CorrelationId));

            if (result.IsSuccess)
            {
                dispatcher.Dispatch(new StartOrderSuccessAction(action.CorrelationId));
                return;
            }

            dispatcher.Dispatch(new StartOrderFailureAction(
                action.CorrelationId,
                result.GenerateErrorMessage()));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new StartOrderFailureAction(
                action.CorrelationId,
                $"Failed to start order: {ex.Message}"));
        }
    }
}
