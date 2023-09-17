using Correlate;
using Fluxor;
using Lewee.Blazor.ErrorHandling;
using Lewee.Blazor.Fluxor;
using Lewee.Shared;
using Microsoft.AspNetCore.Components;
using Sample.Restaurant.Client.States.TableDetails.Actions;

namespace Sample.Restaurant.Client.States.TableDetails;

public sealed class TableDetailsEffects
    : RequestEffects<TableDetailsState, GetTableDetailsAction, GetTableDetailsSuccessAction, GetTableDetailsErrorAction>
{
    private readonly ITableClient tableClient;
    private readonly NavigationManager navigationManager;

    public TableDetailsEffects(
        IState<TableDetailsState> state,
        ITableClient tableClient,
        NavigationManager navigationManager,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<TableDetailsEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.tableClient = tableClient;
        this.navigationManager = navigationManager;
    }

#pragma warning disable IDE0060 // Remove unused parameter (required by Fluxor)
    [EffectMethod]
    public Task NavigateToTableDetails(GetTableDetailsSuccessAction action, IDispatcher dispatcher)
    {
        this.navigationManager.NavigateTo($"tables/{action.Data.TableNumber}");
        return Task.FromResult(true);
    }
#pragma warning restore IDE0060 // Remove unused parameter

    [EffectMethod]
    public async Task OrderItem(OrderItemAction action, IDispatcher dispatcher)
    {
        using (this.Logger.BeginScope(LoggingConsts.CorrelationId, action.CorrelationId.ToString()))
        {
            this.Logger.LogDebug("Ordering menu item...");

            try
            {
                await this.tableClient.OrderMenuItemAsync(action.TableNumber, action.MenuItemId);
                dispatcher.Dispatch(new OrderItemSuccessAction(action.CorrelationId));
            }
            catch (ApiException ex)
            {
                ex.Log(this.Logger);
                dispatcher.Dispatch(new OrderItemErrorAction(ex.Message, action.CorrelationId));
            }
        }
    }

#pragma warning disable IDE0060 // Remove unused parameter
    [EffectMethod]
    public Task OrderItemSuccess(OrderItemSuccessAction action, IDispatcher dispatcher)
    {
        using (this.Logger.BeginScope(LoggingConsts.CorrelationId, action.CorrelationId.ToString()))
        {
            this.Logger.LogDebug("Ordering menu item...success");
            return Task.CompletedTask;
        }
    }

    [EffectMethod]
    public Task OrderItemError(OrderItemErrorAction action, IDispatcher dispatcher)
    {
        using (this.Logger.BeginScope(LoggingConsts.CorrelationId, action.CorrelationId.ToString()))
        {
            // TODO: show error toast
            this.Logger.LogDebug("Ordering menu item...error");
            return Task.CompletedTask;
        }
    }
#pragma warning restore IDE0060 // Remove unused parameter

    [EffectMethod]
    public Task OrderItemCompleted(OrderItemCompletedAction action, IDispatcher dispatcher)
    {
        using (this.Logger.BeginScope(LoggingConsts.CorrelationId, action.CorrelationId.ToString()))
        {
            this.Logger.LogDebug("Received item added message from server");

            dispatcher.Dispatch(new GetTableDetailsAction(action.CorrelationId, action.TableNumber));

            return Task.CompletedTask;
        }
    }

    [EffectMethod]
    public async Task RemoveItem(RemoveItemAction action, IDispatcher dispatcher)
    {
        using (this.Logger.BeginScope(LoggingConsts.CorrelationId, action.CorrelationId.ToString()))
        {
            this.Logger.LogDebug("Removing menu item...");

            try
            {
                await this.tableClient.RemoveMenuItemAsync(action.TableNumber, action.MenuItemId);
                dispatcher.Dispatch(new RemoveItemSuccessAction(action.CorrelationId));
            }
            catch (ApiException ex)
            {
                ex.Log(this.Logger);
                dispatcher.Dispatch(new RemoveItemErrorAction(ex.Message, action.CorrelationId));
            }
        }
    }

#pragma warning disable IDE0060 // Remove unused parameter
    [EffectMethod]
    public Task RemoveItemSuccess(RemoveItemSuccessAction action, IDispatcher dispatcher)
    {
        using (this.Logger.BeginScope(LoggingConsts.CorrelationId, action.CorrelationId.ToString()))
        {
            this.Logger.LogDebug("Removing menu item...success");
            return Task.CompletedTask;
        }
    }

    [EffectMethod]
    public Task RemoveItemError(RemoveItemErrorAction action, IDispatcher dispatcher)
    {
        using (this.Logger.BeginScope(LoggingConsts.CorrelationId, action.CorrelationId.ToString()))
        {
            // TODO: show error toast
            this.Logger.LogDebug("Removing menu item...error");
            return Task.CompletedTask;
        }
    }
#pragma warning restore IDE0060 // Remove unused parameter

    [EffectMethod]
    public Task RemoveItemCompleted(RemoveItemCompletedAction action, IDispatcher dispatcher)
    {
        using (this.Logger.BeginScope(LoggingConsts.CorrelationId, action.CorrelationId.ToString()))
        {
            this.Logger.LogDebug("Received item removed message from server");

            dispatcher.Dispatch(new GetTableDetailsAction(action.CorrelationId, action.TableNumber));

            return Task.CompletedTask;
        }
    }

    protected override async Task ExecuteRequest(GetTableDetailsAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await this.tableClient.GetDetailsAsync(action.TableNumber);
            dispatcher.Dispatch(new GetTableDetailsSuccessAction(result, action.CorrelationId));
        }
        catch (ApiException ex)
        {
            ex.Log(this.Logger);
            dispatcher.Dispatch(new GetTableDetailsErrorAction(ex.Message, action.CorrelationId));
        }
    }
}
