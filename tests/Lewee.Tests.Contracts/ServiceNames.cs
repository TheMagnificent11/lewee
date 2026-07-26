using System.Diagnostics.CodeAnalysis;

namespace Lewee.Tests.Contracts;

[SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "False positive")]
public static class ServiceNames
{
    public const string DatabaseServer = "test-db-server";
    public const string Database = "test-db";
    public const string SignalR = "test-signalr";
    public const string WebApi = "test-webapi";
    public const string BlazorServerWeb = "test-blazor-server-web";
}
