using FluentAssertions;
using Xunit;

namespace Lewee.Domain.Tests.Unit;

public class RelationshipTests
{
    [Fact]
    public void Constructor_CreatesRelationship()
    {
        // Arrange & Act
        var relationship = new TestRelationship();

        // Assert
        relationship.Should().NotBeNull();
        relationship.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Relationship_InheritsFromAuditableRecord()
    {
        // Arrange & Act
        var relationship = new TestRelationship();

        // Assert
        relationship.Should().BeAssignableTo<AuditableRecord>();
    }

    [Fact]
    public void Relationship_HasAuditFields()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var relationship = new TestRelationship();

        // Assert
        relationship.CreatedBy.Should().NotBeNullOrEmpty();
        relationship.ModifiedBy.Should().NotBeNullOrEmpty();
        relationship.CreatedAtUtc.Should().BeOnOrAfter(beforeCreation);
        relationship.ModifiedAtUtc.Should().BeOnOrAfter(beforeCreation);
    }

    private class TestRelationship : Relationship
    {
        public TestRelationship()
            : base()
        {
        }
    }
}
