using System.Diagnostics.CodeAnalysis;
using Fluxor;
using MediatR;
using Pizzeria.Store.Application.Pizzas;
using Pizzeria.Store.Contracts.Pizzas.Actions;

namespace Pizzeria.Store.StateManagement.Pizzas;

public class PizzasEffects
{
    private readonly IMediator mediator;

    public PizzasEffects(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [EffectMethod]
    public async Task OnLoadPizzasAsync(
        LoadPizzasAction action,
        [NotNull] IDispatcher dispatcher)
    {
        ArgumentNullException.ThrowIfNull(action);

        try
        {
            var result = await this.mediator.Send(new GetPizzasQuery(action.CorrelationId));

            if (result.IsSuccess)
            {
                dispatcher.Dispatch(new LoadPizzasSuccessAction(result.Data!));
                return;
            }

            dispatcher.Dispatch(new LoadPizzasFailureAction(result.GenerateErrorMessage()));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoadPizzasFailureAction($"Failed to load pizzas: {ex.Message}"));
        }
    }
}
