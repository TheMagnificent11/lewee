using Fluxor;
using Lewee.Fluxor;
using MediatR;
using Microsoft.AspNetCore.Components;
using Sample.Restaurant.App.States.UseTable.Actions;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.App.States.UseTable;

public sealed class UseTableEffects
    : BaseRequestEffects<UseTableEffects, UseTableState, UseTableAction, UseTableSuccessAction, UseTableErrorAction>
{
    private readonly IMediator mediator;
    private readonly NavigationManager navigationManager;

    public UseTableEffects(
        IState<UseTableState> state,
        IMediator mediator,
        NavigationManager navigationManager,
        Serilog.ILogger logger)
        : base(state, logger)
    {
        this.mediator = mediator;
        this.navigationManager = navigationManager;
    }

    [EffectMethod]
#pragma warning disable IDE0060 // Remove unused parameter (required by Fluxor)
    public Task NavigateToTableDetails(UseTableSuccessAction action, IDispatcher dispatcher)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        this.navigationManager.NavigateTo($"tables/{this.State.Value.TableNumber}");
        return Task.FromResult(true);
    }

    protected override async Task ExecuteRequest(UseTableAction action, IDispatcher dispatcher)
    {
        var result = await this.mediator.Send(new UseTableCommand(action.CorrelationId, action.TableNumber));

        if (result.IsSuccess)
        {
            dispatcher.Dispatch(new UseTableSuccessAction());
            return;
        }

        dispatcher.Dispatch(new UseTableErrorAction(result.GenerateErrorMessage()));
    }
}
