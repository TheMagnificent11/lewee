using Fluxor;
using Pizzeria.Store.WebClient.Services;
using Pizzeria.Store.WebClient.States.Pizzas.Actions;

namespace Pizzeria.Store.WebClient.States.Pizzas;

public class PizzasEffects
{
    private readonly IPizzeriaApiClient apiClient;

    public PizzasEffects(IPizzeriaApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    [EffectMethod]
    public async Task OnLoadPizzas(LoadPizzasAction _, IDispatcher dispatcher)
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