using Xunit;

namespace Lewee.Blazor.Tests.Integration;

[CollectionDefinition(TestServerFixture.CollectionName)]
public sealed class TestCollection : ICollectionFixture<TestServerFixture>
{
}
