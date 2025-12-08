using System.Diagnostics.CodeAnalysis;

namespace Lewee.Blazor.Tests.Contracts;

[SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "False positive")]
public sealed record CreateOrderRequest(string CustomerName, string PizzaType, int Quantity);
