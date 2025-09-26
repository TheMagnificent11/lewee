using Xunit;

namespace Pizzeria.Tests.Integration;

[CollectionDefinition(PizzeriaApplicationFactory.CollectionName)]
public sealed class PizzeriaTestCollection : ICollectionFixture<PizzeriaApplicationFactory>
{
}
