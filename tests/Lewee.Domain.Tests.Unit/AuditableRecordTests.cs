using FluentAssertions;
using Xunit;

namespace Lewee.Domain.Tests.Unit;

public class AuditableRecordTests
{
    [Fact]
    public void DefaultConstructor_GeneratesNewGuid()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var record = new TestAuditableRecord();

        // Assert
        record.Id.Should().NotBeEmpty();
        record.CreatedBy.Should().Be("System");
        record.ModifiedBy.Should().Be("System");
        record.CreatedAtUtc.Should().BeOnOrAfter(beforeCreation);
        record.ModifiedAtUtc.Should().BeOnOrAfter(beforeCreation);
    }

    [Fact]
    public void ConstructorWithId_SetsId()
    {
        // Arrange
        var id = Guid.NewGuid();
        var beforeCreation = DateTime.UtcNow;

        // Act
        var record = new TestAuditableRecord(id);

        // Assert
        record.Id.Should().Be(id);
        record.CreatedBy.Should().Be("System");
        record.ModifiedBy.Should().Be("System");
        record.CreatedAtUtc.Should().BeOnOrAfter(beforeCreation);
        record.ModifiedAtUtc.Should().BeOnOrAfter(beforeCreation);
    }

    [Fact]
    public void ApplyCreationTrackingData_WithUser_SetsAuditFields()
    {
        // Arrange
        var record = new TestAuditableRecord();
        var userId = "user123";
        var beforeUpdate = DateTime.UtcNow;

        // Act
        record.ApplyCreationTrackingData(userId);

        // Assert
        record.CreatedBy.Should().Be(userId);
        record.ModifiedBy.Should().Be(userId);
        record.CreatedAtUtc.Should().BeOnOrAfter(beforeUpdate);
        record.ModifiedAtUtc.Should().BeOnOrAfter(beforeUpdate);
    }

    [Fact]
    public void ApplyCreationTrackingData_WithNull_SetsSystemAsDefault()
    {
        // Arrange
        var record = new TestAuditableRecord();
        var beforeUpdate = DateTime.UtcNow;

        // Act
        record.ApplyCreationTrackingData(null);

        // Assert
        record.CreatedBy.Should().Be("System");
        record.ModifiedBy.Should().Be("System");
        record.CreatedAtUtc.Should().BeOnOrAfter(beforeUpdate);
        record.ModifiedAtUtc.Should().BeOnOrAfter(beforeUpdate);
    }

    [Fact]
    public void ApplyModificationTrackingData_WithUser_UpdatesModifiedFields()
    {
        // Arrange
        var record = new TestAuditableRecord();
        var userId = "user456";
        var beforeUpdate = DateTime.UtcNow;

        // Act
        record.ApplyModificationTrackingData(userId);

        // Assert
        record.ModifiedBy.Should().Be(userId);
        record.ModifiedAtUtc.Should().BeOnOrAfter(beforeUpdate);
    }

    [Fact]
    public void ApplyModificationTrackingData_WithNull_SetsSystemAsDefault()
    {
        // Arrange
        var record = new TestAuditableRecord();
        var beforeUpdate = DateTime.UtcNow;

        // Act
        record.ApplyModificationTrackingData(null);

        // Assert
        record.ModifiedBy.Should().Be("System");
        record.ModifiedAtUtc.Should().BeOnOrAfter(beforeUpdate);
    }

    [Fact]
    public void ApplyCreationTrackingData_UpdatesBothCreatedAndModified()
    {
        // Arrange
        var record = new TestAuditableRecord();
        var userId = "testuser";

        // Act
        record.ApplyCreationTrackingData(userId);

        // Assert
        record.CreatedBy.Should().Be(userId);
        record.ModifiedBy.Should().Be(userId);
        record.CreatedAtUtc.Should().BeCloseTo(record.ModifiedAtUtc, TimeSpan.FromSeconds(1));
    }

    private sealed class TestAuditableRecord : AuditableRecord
    {
        public TestAuditableRecord()
            : base()
        {
        }

        public TestAuditableRecord(Guid id)
            : base(id)
        {
        }
    }
}
