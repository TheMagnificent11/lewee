using Fluxor;
using Pizzeria.Store.WebClient.States.Pizzas.Actions;

namespace Pizzeria.Store.WebClient.States.Pizzas;

public static class PizzasReducer
{
    [ReducerMethod]
    public static PizzasState OnLoadPizzas(PizzasState state, LoadPizzasAction _)
    {
        return state with
        {
            IsLoading = true,
            ErrorMessage = null
        };
    }

    [ReducerMethod]
    public static PizzasState OnLoadPizzasSuccess(PizzasState state, LoadPizzasSuccessAction action)
    {
        return state with
        {
            Pizzas = action.Pizzas,
            IsLoading = false,
            ErrorMessage = null
        };
    }

    [ReducerMethod]
    public static PizzasState OnLoadPizzasFailure(PizzasState state, LoadPizzasFailureAction action)
    {
        return state with
        {
            IsLoading = false,
            ErrorMessage = action.ErrorMessage
        };
    }
}