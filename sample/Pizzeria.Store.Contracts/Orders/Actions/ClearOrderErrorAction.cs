using System.Diagnostics.CodeAnalysis;

namespace Pizzeria.Store.Contracts.Orders.Actions;

[SuppressMessage(
    "Minor Code Smell",
    "S2094:Classes should not be empty",
    Justification = "False positive")]
public record ClearOrderErrorAction;
