using System.Diagnostics.CodeAnalysis;

namespace Lewee.Blazor.Tests.Integration;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Used via DI")]
internal sealed record CreateOrderRequest(string CustomerName, string PizzaType, int Quantity);
