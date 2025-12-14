using System.Diagnostics.CodeAnalysis;
using Fluxor;
using Pizzeria.Store.StateManagement.Pizzas.Actions;

namespace Pizzeria.Store.StateManagement.Pizzas;

public static class PizzasReducer
{
    [ReducerMethod]
    public static PizzasState OnLoadPizzas(
        [NotNull] PizzasState state,
        [NotNull] LoadPizzasAction _)
    {
        ArgumentNullException.ThrowIfNull(state);

        return state with
        {
            IsLoading = true,
            ErrorMessage = null,
        };
    }

    [ReducerMethod]
    public static PizzasState OnLoadPizzasSuccess(
        [NotNull] PizzasState state,
        [NotNull] LoadPizzasSuccessAction action)
    {
        return state with
        {
            Pizzas = action.Data,
            IsLoading = false,
            ErrorMessage = null,
        };
    }

    [ReducerMethod]
    public static PizzasState OnLoadPizzasFailure(
        [NotNull] PizzasState state,
        [NotNull] LoadPizzasFailureAction action)
    {
        return state with
        {
            IsLoading = false,
            ErrorMessage = action.ErrorMessage,
        };
    }
}
