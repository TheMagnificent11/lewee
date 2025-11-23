using System.Diagnostics.CodeAnalysis;

namespace Pizzeria.Store.Api.Customers;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Used via DI")]
internal record CreateCustomerRequest
{
    public string ExternalUserId { get; init; }
}
