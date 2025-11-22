using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.Extensions.Logging.Testing;

namespace Lewee.Tests.Common;

[SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "Intended to be used by other test projects")]
public static class LogEntryExtensions
{
    public static void ShouldHaveScope(this FakeLogRecord logEntry, string scopeName, object scopeValue)
    {
        ArgumentNullException.ThrowIfNull(logEntry);
        ArgumentNullException.ThrowIfNull(scopeValue);

        var scopeDict = logEntry.Scopes.Cast<IEnumerable<KeyValuePair<string, object>>>().FirstOrDefault();
        scopeDict.Should().NotBeNull();

        var scope = scopeDict.FirstOrDefault(kvp => string.Equals(kvp.Key, scopeName, StringComparison.Ordinal));
        scope.Should().NotBeNull();
        scope.Value.ToString().Should().Be(scopeValue.ToString());
    }
}
