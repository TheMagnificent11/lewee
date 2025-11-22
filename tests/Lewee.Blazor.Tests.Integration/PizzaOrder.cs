using System.Diagnostics.CodeAnalysis;

namespace Lewee.Blazor.Tests.Integration;

[SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "Used by test classes that need to be public")]
public sealed record PizzaOrder
{
    public Guid Id { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
