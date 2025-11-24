using Fluxor;
using Pizzeria.Store.Web.States.Pizzas.Actions;

namespace Pizzeria.Store.Web.States.Pizzas;

public static class PizzasReducer
{
    [ReducerMethod]
    public static PizzasState OnLoadPizzas(PizzasState state, LoadPizzasAction _)
    {
        ArgumentNullException.ThrowIfNull(state);

        return state with
        {
            IsLoading = true,
            ErrorMessage = null,
        };
    }

    [ReducerMethod]
    public static PizzasState OnLoadPizzasSuccess(PizzasState state, LoadPizzasSuccessAction action)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);

        return state with
        {
            Pizzas = action.Pizzas,
            IsLoading = false,
            ErrorMessage = null,
        };
    }

    [ReducerMethod]
    public static PizzasState OnLoadPizzasFailure(PizzasState state, LoadPizzasFailureAction action)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);

        return state with
        {
            IsLoading = false,
            ErrorMessage = action.ErrorMessage,
        };
    }
}
