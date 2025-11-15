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
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter '_' should begin with lower-case letter", Justification = "Underscore is the standard discard pattern for unused parameters")]
    public async Task OnLoadPizzasAsync(LoadPizzasAction _, IDispatcher dispatcher)
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
