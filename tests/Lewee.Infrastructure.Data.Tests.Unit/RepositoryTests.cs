using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Lewee.Domain;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Lewee.Infrastructure.Data.Tests.Unit;

public class RepositoryTests
{
    [Fact]
    public async Task QueryAsync_WithWhereSpecification_ShouldApplyFilterAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TestDb_QueryAsync_Where")
            .Options;

        await using var context = new TestDbContext(options);
        var entity1 = new TestEntity { Name = "Test1" };
        var entity2 = new TestEntity { Name = "Test2" };
        await context.TestEntities.AddRangeAsync(entity1, entity2);
        await context.SaveChangesAsync();

        var repository = new Repository<TestEntity, TestDbContext>(context);
        var spec = new TestWhereSpecification(entity1.Id);

        // Act
        var results = await repository.QueryAsync(spec);

        // Assert
        results.Should().ContainSingle();
        results[0].Id.Should().Be(entity1.Id);
    }

    [Fact]
    public async Task QueryAsync_WithMultipleWhereSpecification_ShouldApplyAllFiltersAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TestDb_QueryAsync_MultipleWhere")
            .Options;

        await using var context = new TestDbContext(options);
        var entity1 = new TestEntity { Name = "Test1" };
        var entity2 = new TestEntity { Name = "Test2" };
        var entity3 = new TestEntity { Name = "Test3" };
        await context.TestEntities.AddRangeAsync(entity1, entity2, entity3);
        await context.SaveChangesAsync();

        var repository = new Repository<TestEntity, TestDbContext>(context);
        var spec = new TestMultipleWhereSpecification("Test1");

        // Act
        var results = await repository.QueryAsync(spec);

        // Assert
        results.Should().ContainSingle();
        results[0].Name.Should().Be("Test1");
    }

    [Fact]
    public async Task QueryOneAsync_WithWhereSpecification_ShouldReturnFirstMatchAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TestDb_QueryOneAsync")
            .Options;

        await using var context = new TestDbContext(options);
        var entity1 = new TestEntity { Name = "Test1" };
        var entity2 = new TestEntity { Name = "Test2" };
        await context.TestEntities.AddRangeAsync(entity1, entity2);
        await context.SaveChangesAsync();

        var repository = new Repository<TestEntity, TestDbContext>(context);
        var spec = new TestWhereSpecification(entity1.Id);

        // Act
        var result = await repository.QueryOneAsync(spec);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(entity1.Id);
    }

    [Fact]
    public async Task QueryOneAsync_WithNoMatches_ShouldReturnNullAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TestDb_QueryOneAsync_NoMatches")
            .Options;

        await using var context = new TestDbContext(options);
        var entity1 = new TestEntity { Name = "Test1" };
        context.TestEntities.Add(entity1);
        await context.SaveChangesAsync();

        var repository = new Repository<TestEntity, TestDbContext>(context);
        var spec = new TestWhereSpecification(Guid.NewGuid());

        // Act
        var result = await repository.QueryOneAsync(spec);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AllAsync_ShouldReturnAllEntitiesAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TestDb_AllAsync")
            .Options;

        await using var context = new TestDbContext(options);
        var entity1 = new TestEntity { Name = "Test1" };
        var entity2 = new TestEntity { Name = "Test2" };
        await context.TestEntities.AddRangeAsync(entity1, entity2);
        await context.SaveChangesAsync();

        var repository = new Repository<TestEntity, TestDbContext>(context);

        // Act
        var results = await repository.AllAsync();

        // Assert
        results.Should().HaveCount(2);
    }

    [Fact]
    public async Task RetrieveByIdAsync_WithValidId_ShouldReturnEntityAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TestDb_RetrieveByIdAsync")
            .Options;

        await using var context = new TestDbContext(options);
        var entity = new TestEntity { Name = "Test1" };
        context.TestEntities.Add(entity);
        await context.SaveChangesAsync();

        var repository = new Repository<TestEntity, TestDbContext>(context);

        // Act
        var result = await repository.RetrieveByIdAsync(entity.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
    }

    [Fact]
    public async Task RetrieveByIdAsync_WithInvalidId_ShouldReturnNullAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TestDb_RetrieveByIdAsync_Invalid")
            .Options;

        await using var context = new TestDbContext(options);
        var repository = new Repository<TestEntity, TestDbContext>(context);

        // Act
        var result = await repository.RetrieveByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntityAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TestDb_AddAsync")
            .Options;

        await using var context = new TestDbContext(options);
        var repository = new Repository<TestEntity, TestDbContext>(context);
        var entity = new TestEntity { Name = "Test1" };

        // Act
        await repository.AddAsync(entity);
        await repository.SaveChangesAsync();

        // Assert
        var allEntities = await context.TestEntities.ToListAsync();
        allEntities.Should().ContainSingle();
        allEntities[0].Name.Should().Be("Test1");
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistChangesAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TestDb_SaveChangesAsync")
            .Options;

        await using var context = new TestDbContext(options);
        var repository = new Repository<TestEntity, TestDbContext>(context);
        var entity = new TestEntity { Name = "Test1" };

        // Act
        await repository.AddAsync(entity);
        var changeCount = await repository.SaveChangesAsync();

        // Assert
        changeCount.Should().Be(1);
    }

    private sealed class TestEntity : AggregateRoot
    {
        public string Name { get; set; } = string.Empty;
    }

    private sealed class TestWhereSpecification : QuerySpecification<TestEntity>
    {
        public TestWhereSpecification(Guid id)
        {
            this.Query.Where(x => x.Id == id);
        }
    }

    private sealed class TestMultipleWhereSpecification : QuerySpecification<TestEntity>
    {
        public TestMultipleWhereSpecification(string name)
        {
            this.Query
                .Where(x => x.Name == name)
                .Where(x => x.Id != Guid.Empty);
        }
    }

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "False positive")]
    private sealed class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        [SuppressMessage(
            "Major Code Smell",
            "S1144:Unused private types or members should be removed",
            Justification = "Needed for EF")]
        public DbSet<TestEntity> TestEntities { get; set; }
    }
}
