using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Lewee.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Xunit;

namespace Lewee.Infrastructure.Data.Tests.Unit;

public class AuditableRecordConfigurationTests
{
    [Fact]
    public void Configure_WithPostgreSql_ShouldAddConcurrencyTokenWithXidType()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestPostgreSqlContext>()
            .UseNpgsql("Host=localhost;Database=test;Username=test;Password=test")
            .Options;

        using var context = new TestPostgreSqlContext(options);
        var model = context.Model;

        // Act
        var entityType = model.FindEntityType(typeof(TestEntity));

        // Assert
        entityType.Should().NotBeNull();
        var versionProperty = entityType.FindProperty("Version");
        versionProperty.Should().NotBeNull();
        versionProperty.ClrType.Should().Be<uint>();
        versionProperty.IsConcurrencyToken.Should().BeTrue();
        versionProperty.GetColumnType().Should().Be("xid");
    }

    [Fact]
    public void Configure_WithSqlServer_ShouldAddConcurrencyTokenWithRowVersion()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestSqlServerContext>()
            .UseSqlServer("Server=localhost;Database=test;User Id=test;Password=test;TrustServerCertificate=true")
            .Options;

        using var context = new TestSqlServerContext(options);
        var model = context.Model;

        // Act
        var entityType = model.FindEntityType(typeof(TestEntity));

        // Assert
        entityType.Should().NotBeNull();
        var versionProperty = entityType.FindProperty("Version");
        versionProperty.Should().NotBeNull();
        versionProperty.ClrType.Should().Be<byte[]>();
        versionProperty.IsConcurrencyToken.Should().BeTrue();
    }

    [Fact]
    public void Configure_WithInMemory_ShouldNotAddConcurrencyToken()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestInMemoryContext>()
            .UseInMemoryDatabase("test")
            .Options;

        using var context = new TestInMemoryContext(options);
        var model = context.Model;

        // Act
        var entityType = model.FindEntityType(typeof(TestEntity));

        // Assert
        entityType.Should().NotBeNull();
        var versionProperty = entityType.FindProperty("Version");
        versionProperty.Should().BeNull("because InMemory provider is not PostgreSQL or SQL Server");
    }

    [Fact]
    public void Configure_ShouldCallAddAuditUserProperties()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestPostgreSqlContext>()
            .UseNpgsql("Host=localhost;Database=test;Username=test;Password=test")
            .Options;

        using var context = new TestPostgreSqlContext(options);
        var model = context.Model;

        // Act
        var entityType = model.FindEntityType(typeof(TestEntity));

        // Assert
        entityType.Should().NotBeNull();
        var createdByProperty = entityType.FindProperty(nameof(AuditableRecord.CreatedBy));
        createdByProperty.Should().NotBeNull();
        createdByProperty.IsNullable.Should().BeFalse();
        createdByProperty.GetMaxLength().Should().Be(255);

        var modifiedByProperty = entityType.FindProperty(nameof(AuditableRecord.ModifiedBy));
        modifiedByProperty.Should().NotBeNull();
        modifiedByProperty.IsNullable.Should().BeFalse();
        modifiedByProperty.GetMaxLength().Should().Be(255);
    }

    [Fact]
    public void Configure_ShouldSetPrimaryKey()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestPostgreSqlContext>()
            .UseNpgsql("Host=localhost;Database=test;Username=test;Password=test")
            .Options;

        using var context = new TestPostgreSqlContext(options);
        var model = context.Model;

        // Act
        var entityType = model.FindEntityType(typeof(TestEntity));

        // Assert
        entityType.Should().NotBeNull();
        var primaryKey = entityType.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey.Properties.Should().ContainSingle();
        primaryKey.Properties[0].Name.Should().Be("Id");
    }

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "False positive")]
    private sealed class TestEntity : AuditableRecord
    {
        public string Name { get; set; } = string.Empty;
    }

    private sealed class TestEntityConfiguration : AuditableRecordConfiguration<TestEntity>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<TestEntity> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }

    private sealed class TestPostgreSqlContext : DbContext
    {
        public TestPostgreSqlContext(DbContextOptions<TestPostgreSqlContext> options)
            : base(options)
        {
        }

        [SuppressMessage(
            "Major Code Smell",
            "S1144:Unused private types or members should be removed",
            Justification = "Needed for EF")]
        public DbSet<TestEntity> TestEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new TestEntityConfiguration());
        }
    }

    private sealed class TestSqlServerContext : DbContext
    {
        public TestSqlServerContext(DbContextOptions<TestSqlServerContext> options)
            : base(options)
        {
        }

        [SuppressMessage(
            "Major Code Smell",
            "S1144:Unused private types or members should be removed",
            Justification = "Needed for EF")]
        public DbSet<TestEntity> TestEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new TestEntityConfiguration());
        }
    }

    private sealed class TestInMemoryContext : DbContext
    {
        public TestInMemoryContext(DbContextOptions<TestInMemoryContext> options)
            : base(options)
        {
        }

        [SuppressMessage(
            "Major Code Smell",
            "S1144:Unused private types or members should be removed",
            Justification = "Needed for EF")]
        public DbSet<TestEntity> TestEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new TestEntityConfiguration());
        }
    }
}
