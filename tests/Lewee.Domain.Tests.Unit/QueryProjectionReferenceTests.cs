using FluentAssertions;
using Xunit;

namespace Lewee.Domain.Tests.Unit;

public class QueryProjectionReferenceTests
{
    [Fact]
    public void Constructor_InitializesProperties()
    {
        // Arrange
        var projection = new TestQueryProjection(Guid.NewGuid(), "Test Name", 42);
        var key = "test-key";
        var beforeCreation = DateTime.UtcNow;

        // Act
        var reference = new QueryProjectionReference(projection, key);

        // Assert
        reference.Should().NotBeNull();
        reference.Id.Should().NotBeEmpty();
        reference.Key.Should().Be(key);
        reference.QueryProjectionAssemblyName.Should().Be(projection.GetType().Assembly.GetName().Name);
        reference.QueryProjectionClassName.Should().Be(projection.GetType().FullName);
        reference.QueryProjectionJson.Should().NotBeNullOrEmpty();
        reference.CreatedAtUtc.Should().BeOnOrAfter(beforeCreation);
        reference.ModifiedAtUtc.Should().BeOnOrAfter(beforeCreation);
        reference.IsDeleted.Should().BeFalse();
        reference.Version.Should().Be(0);
    }

    [Fact]
    public void ToQueryProjection_ReturnsOriginalProjection()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var name = "Test Name";
        var count = 42;
        var projection = new TestQueryProjection(correlationId, name, count);
        var key = "test-key";
        var reference = new QueryProjectionReference(projection, key);

        // Act
        var result = reference.ToQueryProjection();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<TestQueryProjection>();
        var typedResult = (TestQueryProjection)result;
        typedResult.CorrelationId.Should().Be(correlationId);
        typedResult.Name.Should().Be(name);
        typedResult.Count.Should().Be(count);
    }

    [Fact]
    public void UpdateJson_UpdatesJsonAndModifiedTime()
    {
        // Arrange
        var projection1 = new TestQueryProjection(Guid.NewGuid(), "Name1", 1);
        var key = "test-key";
        var reference = new QueryProjectionReference(projection1, key);
        var originalJson = reference.QueryProjectionJson;

        var projection2 = new TestQueryProjection(Guid.NewGuid(), "Name2", 2);

        // Act
        reference.UpdateJson(projection2);

        // Assert
        reference.QueryProjectionJson.Should().NotBe(originalJson);

        var result = reference.ToQueryProjection();
        result.Should().NotBeNull();
        var typedResult = (TestQueryProjection)result;
        typedResult.Name.Should().Be("Name2");
        typedResult.Count.Should().Be(2);
    }

    [Fact]
    public void ISoftDeleteEntity_IsDeleted_DefaultsToFalse()
    {
        // Arrange
        var projection = new TestQueryProjection(Guid.NewGuid(), "Test", 1);
        var key = "test-key";

        // Act
        var reference = new QueryProjectionReference(projection, key);

        // Assert
        reference.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Version_DefaultsToZero()
    {
        // Arrange
        var projection = new TestQueryProjection(Guid.NewGuid(), "Test", 1);
        var key = "test-key";

        // Act
        var reference = new QueryProjectionReference(projection, key);

        // Assert
        reference.Version.Should().Be(0);
    }

    private sealed class TestQueryProjection : IQueryProjection
    {
        public TestQueryProjection(Guid correlationId, string name, int count)
        {
            this.CorrelationId = correlationId;
            this.Name = name;
            this.Count = count;
        }

        public Guid CorrelationId { get; }

        public string Name { get; }

        public int Count { get; }
    }
}
