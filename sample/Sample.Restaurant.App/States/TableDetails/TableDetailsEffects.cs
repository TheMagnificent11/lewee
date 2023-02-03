using Fluxor;
using Lewee.Fluxor;
using MediatR;
using Microsoft.AspNetCore.Components;
using Sample.Restaurant.App.States.TableDetails.Actions;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.App.States.TableDetails;

public sealed class TableDetailsEffects
    : BaseQueryEffects<TableDetailsEffects, TableDetailsState, TableDetailsDto, GetTableDetails, GetTableDetailsSuccess, GetTableDetailsError>
{
    private readonly IMediator mediator;
    private readonly NavigationManager navigationManager;

    public TableDetailsEffects(
        IState<TableDetailsState> state,
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
    public Task NavigateToTableDetails(GetTableDetailsSuccess action, IDispatcher dispatcher)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        this.navigationManager.NavigateTo($"tables/{action.Data.TableNumber}");
        return Task.FromResult(true);
    }

    protected override async Task ExecuteQuery(GetTableDetails action, IDispatcher dispatcher)
    {
        var result = await this.mediator.Send(new GetTableDetailsQuery(action.CorrelationId, action.TableNumber));

        if (result.IsSuccess && result.Data != null)
        {
            dispatcher.Dispatch(new GetTableDetailsSuccess(result.Data));
            return;
        }

        dispatcher.Dispatch(new GetTableDetailsError(result.GenerateErrorMessage()));
    }
}
