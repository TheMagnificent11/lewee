using System.Diagnostics.CodeAnalysis;
using Fluxor;
using Pizzeria.Store.Web.States.Pizzas.Actions;

namespace Pizzeria.Store.Web.States.Pizzas;

public static class PizzasReducer
{
    [ReducerMethod]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter '_' should begin with lower-case letter", Justification = "Underscore is the standard discard pattern for unused parameters")]
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
