using System.Diagnostics.CodeAnalysis;

namespace Pizzeria.Store.Contracts;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Used via DI")]
public record CreateCustomerRequest
{
    public string ExternalUserId { get; init; }
}
