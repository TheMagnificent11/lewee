using FluentAssertions;
using Lewee.Domain;
using Lewee.Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Lewee.Infrastructure.Data.Tests.Unit;

public class PostgresApplicationDbContextTests
{
    [Fact]
    public void Constructor_ShouldCreateContextSuccessfully()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestPostgresContext>()
            .UseNpgsql("Host=localhost;Database=test;Username=test;Password=test")
            .Options;

        // Act
        using var context = new TestPostgresContext(options);

        // Assert
        context.Should().NotBeNull();
        context.Should().BeAssignableTo<PostgresApplicationDbContext<TestPostgresContext>>();
        context.Should().BeAssignableTo<ApplicationDbContext<TestPostgresContext>>();
    }

    [Fact]
    public void Schema_ShouldReturnConfiguredSchema()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestPostgresContext>()
            .UseNpgsql("Host=localhost;Database=test;Username=test;Password=test")
            .Options;

        using var context = new TestPostgresContext(options);

        // Act
        var schema = context.Schema;

        // Assert
        schema.Should().Be("test_schema");
    }

    [Fact]
    public void Model_ShouldIncludeDomainEventReferences()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestPostgresContext>()
            .UseNpgsql("Host=localhost;Database=test;Username=test;Password=test")
            .Options;

        using var context = new TestPostgresContext(options);
        var model = context.Model;

        // Act
        var entityType = model.FindEntityType(typeof(DomainEventReference));

        // Assert
        entityType.Should().NotBeNull("because DomainEventReference should be configured in ApplicationDbContext");
    }

    [Fact]
    public void Model_ShouldIncludeQueryProjectionReferences()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestPostgresContext>()
            .UseNpgsql("Host=localhost;Database=test;Username=test;Password=test")
            .Options;

        using var context = new TestPostgresContext(options);
        var model = context.Model;

        // Act
        var entityType = model.FindEntityType(typeof(QueryProjectionReference));

        // Assert
        entityType.Should().NotBeNull("because QueryProjectionReference should be configured in ApplicationDbContext");
    }

    // Test context
    private class TestPostgresContext : PostgresApplicationDbContext<TestPostgresContext>
    {
        public TestPostgresContext(DbContextOptions<TestPostgresContext> options)
            : base(options)
        {
        }

        public override string Schema => "test_schema";

        public DbSet<TestEntity> TestEntities { get; set; } = null!;
    }

    // Test entity
    private class TestEntity : AuditableRecord
    {
        public string Name { get; set; } = string.Empty;
    }
}
