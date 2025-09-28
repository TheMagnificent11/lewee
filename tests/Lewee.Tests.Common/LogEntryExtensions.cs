using FluentAssertions;
using Microsoft.Extensions.Logging.Testing;

namespace Lewee.Tests.Common;

/// <summary>
/// Extension methods for testing log entries
/// </summary>
public static class LogEntryExtensions
{
    /// <summary>
    /// Asserts that a log entry has a specific scope with the expected value
    /// </summary>
    /// <param name="logEntry">The log entry to check</param>
    /// <param name="scopeName">The name of the scope key</param>
    /// <param name="scopeValue">The expected scope value</param>
    public static void ShouldHaveScope(this FakeLogRecord logEntry, string scopeName, object scopeValue)
    {
        var scopeDict = logEntry.Scopes.Cast<IEnumerable<KeyValuePair<string, object>>>().FirstOrDefault();
        scopeDict.Should().NotBeNull();
        
        var scope = scopeDict.FirstOrDefault(kvp => kvp.Key == scopeName);
        scope.Should().NotBeNull();
        scope.Value.ToString().Should().Be(scopeValue.ToString());
    }
}