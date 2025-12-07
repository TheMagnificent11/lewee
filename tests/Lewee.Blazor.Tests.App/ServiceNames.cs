using System.Diagnostics.CodeAnalysis;

namespace Lewee.Blazor.Tests.App;

[SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "False positive")]
public static class ServiceNames
{
    public const string SignalR = "test-signalr";
}
