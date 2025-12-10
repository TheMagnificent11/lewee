using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Lewee.Blazor.Tests.Integration.Infrastructure;

[CollectionDefinition(BlazorServerTestFixture.CollectionName)]
[SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "xUnit requires this to be public")]
public sealed class BlazorServerTestCollection : ICollectionFixture<BlazorServerTestFixture>
{
}
