using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Lewee.Blazor.Tests.Integration;

[SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "xUnit requires this to be public")]
[CollectionDefinition(TestFixture.CollectionName)]
public sealed class TestCollection : ICollectionFixture<TestFixture>
{
}
