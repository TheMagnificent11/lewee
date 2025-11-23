using System.Diagnostics.CodeAnalysis;

namespace Pizzeria.Store.Web.States.Pizzas.Actions;

[SuppressMessage(
    "Minor Code Smell",
    "S2094:Classes should not be empty",
    Justification = "False positive")]
public record LoadPizzasAction;
