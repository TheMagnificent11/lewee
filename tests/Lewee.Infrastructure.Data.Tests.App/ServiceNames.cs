using System.Diagnostics.CodeAnalysis;

namespace Lewee.Infrastructure.Data.Tests.App;

[SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "False positive")]
public static class ServiceNames
{
    public const string DatabaseServer = "test-db-server";
    public const string Database = "test-db";
}
