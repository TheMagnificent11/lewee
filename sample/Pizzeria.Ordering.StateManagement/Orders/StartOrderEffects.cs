using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.Common;
using Lewee.Infrastructure.Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Pizzeria.Ordering.StateManagement.Orders.Actions;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.Contracts.Orders;

namespace Pizzeria.Ordering.StateManagement.Orders;

public sealed class StartOrderEffects :
    CommandEffects<OrderState, OrderDto, StartOrderAction, StartOrderSuccessAction, StartOrderFailureAction, StartOrderCompletedAction>
{
    private readonly IBffApiClient bffApiClient;
    private readonly NavigationManager navigationManager;
    private readonly IOrderCommandCorrelationTracker correlationTracker;

    public StartOrderEffects(
        IState<OrderState> state,
        IBffApiClient bffApiClient,
        NavigationManager navigationManager,
        IOrderCommandCorrelationTracker correlationTracker,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<StartOrderEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.bffApiClient = bffApiClient;
        this.navigationManager = navigationManager;
        this.correlationTracker = correlationTracker;
    }

    protected override async Task<CommandResult> ExecuteCommandAsync(
        [NotNull] StartOrderAction action,
        [NotNull] IDispatcher dispatcher)
    {
        this.correlationTracker.Register(action.CorrelationId, OrderCommandType.StartOrder);

        await this.bffApiClient.StartOrderAsync();

        return CommandResult.Success();
    }

    protected override Task ExecuteCommandCompletedAsync(
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
