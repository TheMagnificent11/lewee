using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Pizzeria.Tests.Integration;

[SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "xUnit requires this to be public")]
[CollectionDefinition(PizzeriaApplicationFactory.CollectionName)]
public sealed class PizzeriaTestCollection : ICollectionFixture<PizzeriaApplicationFactory>
{
}
