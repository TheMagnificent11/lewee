using System.Diagnostics.CodeAnalysis;

namespace Pizzeria.Auth;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Used via DI")]
internal sealed class KeycloakOptions
{
    public string RealmName { get; set; } = string.Empty;
}
