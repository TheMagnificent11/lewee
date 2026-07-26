using FluentAssertions;
using Xunit;

namespace Lewee.Domain.Tests.Unit;

public class AggregateRootTests
{
    [Fact]
    public void DefaultConstructor_GeneratesNewGuid()
    {
        // Arrange & Act
        var aggregate = new TestAggregate();

        // Assert
        aggregate.Id.Should().NotBeEmpty();
        aggregate.DomainEvents.Should().NotBeNull();
    }

    [Fact]
    public void ConstructorWithId_SetsId()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var aggregate = new TestAggregate(id);

        // Assert
        aggregate.Id.Should().Be(id);
        aggregate.DomainEvents.Should().NotBeNull();
    }

    [Fact]
    public void DomainEventsCollection_IsInitialized()
    {
        // Arrange & Act
        var aggregate = new TestAggregate();

        // Assert
        aggregate.DomainEvents.Should().NotBeNull();
        aggregate.DomainEvents.Should().BeOfType<DomainEventsCollection>();
    }

    private sealed class TestAggregate : AggregateRoot
    {
        public TestAggregate()
            : base()
        {
        }

        public TestAggregate(Guid id)
            : base(id)
        {
        }
    }
}
