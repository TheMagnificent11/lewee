using Xunit;

namespace Lewee.Blazor.Tests.Integration;

[CollectionDefinition(TestFixture.CollectionName)]
public sealed class TestCollection : ICollectionFixture<TestFixture>
{
}
