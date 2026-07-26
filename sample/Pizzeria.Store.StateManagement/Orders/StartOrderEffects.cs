using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.Common;
using Lewee.Infrastructure.Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.StateManagement.Orders.Actions;

namespace Pizzeria.Store.StateManagement.Orders;

public sealed class StartOrderEffects :
    CommandEffects<OrderState, OrderDto, StartOrderAction, StartOrderSuccessAction, StartOrderFailureAction, StartOrderCompletedAction>
{
    private readonly IStoreApiClient storeApiClient;
    private readonly NavigationManager navigationManager;

    public StartOrderEffects(
        IState<OrderState> state,
        IStoreApiClient storeApiClient,
        NavigationManager navigationManager,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<StartOrderEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.storeApiClient = storeApiClient;
        this.navigationManager = navigationManager;
    }

    protected override async Task<CommandResult> ExecuteCommandAsync(
        [NotNull] StartOrderAction action,
        [NotNull] IDispatcher dispatcher)
    {
        await this.storeApiClient.StartOrderAsync();

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
