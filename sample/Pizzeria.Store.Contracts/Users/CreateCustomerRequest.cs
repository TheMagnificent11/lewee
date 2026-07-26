using System.Diagnostics.CodeAnalysis;

namespace Pizzeria.Store.Contracts.Users;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Used via DI")]
public sealed record CreateCustomerRequest
{
    public string ExternalUserId { get; init; }
}
