using System.Diagnostics.CodeAnalysis;
namespace Lewee.Tests.Contracts;

[SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "False positive")]
public record AddPizzaToMenuRequest(
    string Name,
    string Description,
    decimal Price);
