using System.Diagnostics.CodeAnalysis;

namespace Pizzeria.Store.Web.Pizzas.Actions;

[SuppressMessage(
    "Minor Code Smell",
    "S2094:Classes should not be empty",
    Justification = "False positive")]
public record LoadPizzasAction;
