using Microsoft.EntityFrameworkCore;

namespace Lewee.Infrastructure.Data.Tests.Integration;

/// <summary>
/// Test database context for integration testing
/// </summary>
internal sealed class TestDbContext : ApplicationDbContext<TestDbContext>
{
    public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options)
    {
    }

    public DbSet<TestOrder> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("test");

        modelBuilder.Entity<TestOrder>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(100);
        });
    }
}
