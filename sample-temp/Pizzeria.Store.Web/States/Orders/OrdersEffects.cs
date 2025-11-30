using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.Blazor.Fluxor;
using Microsoft.AspNetCore.Components;
using Pizzeria.Store.Web.Services;
using Pizzeria.Store.Web.States.Orders.Actions;

namespace Pizzeria.Store.Web.States.Orders;

public class OrdersEffects : RequestEffects<OrdersState, StartOrderAction, StartOrderSuccessAction, StartOrderFailureAction>
{
    private readonly IPizzeriaApiClient apiClient;
    private readonly NavigationManager navigationManager;

    public OrdersEffects(
        IState<OrdersState> state,
        IPizzeriaApiClient apiClient,
        NavigationManager navigationManager,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<OrdersEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.apiClient = apiClient;
        this.navigationManager = navigationManager;
    }

    [EffectMethod]
    public Task OnStartOrderCompletedAsync(
        [NotNull] StartOrderCompletedAction action,
        [NotNull] IDispatcher _)
    {
        if (action.Order is null || action.Order.Id == Guid.Empty)
        {
            this.Logger?.LogWarning("Order ID is null or empty. Navigation to order details page aborted.");
            return Task.CompletedTask;
        }

        this.navigationManager.NavigateTo($"/orders/{action.Order.Id}");

        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task OnAddPizzaToOrderAsync(
        [NotNull] AddPizzaToOrderAction action,
        [NotNull] IDispatcher dispatcher)
    {
        try
        {
            await this.apiClient.AddPizzaToOrderAsync(action.OrderId, action.PizzaId);
            dispatcher.Dispatch(new AddPizzaToOrderSuccessAction());
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new AddPizzaToOrderFailureAction(action.PizzaId, $"Failed to add pizza: {ex.Message}"));
        }
    }

    protected override async Task ExecuteRequestAsync(
        [NotNull] StartOrderAction action,
        [NotNull] IDispatcher dispatcher)
    {
        try
        {
            await this.apiClient.StartOrderAsync();
            dispatcher.Dispatch(new StartOrderSuccessAction { CorrelationId = action.CorrelationId });
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new StartOrderFailureAction(action.CorrelationId, $"Failed to start order: {ex.Message}"));
        }
    }
}
