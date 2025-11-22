using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.Blazor.Fluxor;
using Microsoft.AspNetCore.Components;
using Pizzeria.Store.Web.Services;
using Pizzeria.Store.Web.States.Orders.Actions;

namespace Pizzeria.Store.Web.States.Orders;

internal class OrdersEffects : RequestEffects<OrdersState, StartOrderAction, StartOrderSuccessAction, StartOrderFailureAction>
{
    private readonly IPizzeriaApiClient apiClient;
    private readonly NavigationManager navigationManager;

    public OrdersEffects(
        IState<OrdersState> state,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<OrdersEffects> logger,
        IPizzeriaApiClient apiClient,
        NavigationManager navigationManager)
        : base(state, correlationContextAccessor, logger)
    {
        this.apiClient = apiClient;
        this.navigationManager = navigationManager;
    }

    [EffectMethod]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter '_' should begin with lower-case letter", Justification = "Underscore is the standard discard pattern for unused parameters")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:'public' members should come before 'protected' members", Justification = "Fluxor EffectMethod must be public")]
    public Task OnStartOrderCompletedAsync(StartOrderCompletedAction _, IDispatcher __)
    {
        this.navigationManager.NavigateTo(Routes.Order);
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task OnAddPizzaToOrderAsync(AddPizzaToOrderAction action, IDispatcher dispatcher)
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

    protected override async Task ExecuteRequestAsync(StartOrderAction action, IDispatcher dispatcher)
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
