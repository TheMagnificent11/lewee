using System.Diagnostics.CodeAnalysis;
using Fluxor;
using Pizzeria.Store.Web.Services;
using Pizzeria.Store.Web.States.Pizzas.Actions;

namespace Pizzeria.Store.Web.States.Pizzas;

public class PizzasEffects
{
    private readonly IPizzeriaApiClient apiClient;

    public PizzasEffects(IPizzeriaApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    [EffectMethod]
    public async Task OnLoadPizzasAsync(
        LoadPizzasAction _,
        [NotNull] IDispatcher dispatcher)
    {
        try
        {
            var pizzas = await this.apiClient.GetPizzasAsync();
            dispatcher.Dispatch(new LoadPizzasSuccessAction(pizzas));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoadPizzasFailureAction($"Failed to load pizzas: {ex.Message}"));
        }
    }
}
