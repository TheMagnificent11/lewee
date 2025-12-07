using System.Diagnostics.CodeAnalysis;

namespace Lewee.Blazor.Tests.Contracts;

[SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "Used by test classes that need to be public")]
public sealed record PizzaOrder(Guid Id, string CustomerName, DateTime CreatedAt);
