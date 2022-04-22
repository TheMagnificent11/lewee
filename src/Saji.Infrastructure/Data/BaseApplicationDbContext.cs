using Microsoft.EntityFrameworkCore;

namespace Saji.Infrastructure.Data;

/// <summary>
/// Base Appliation Database Context
/// </summary>
public abstract class BaseApplicationDbContext : DbContext
{
    /// <summary>
    /// Gets the database schema for the context
    /// </summary>
    protected abstract string Schema { get; }

    /// <summary>
    /// Configures the database
    /// </summary>
    /// <param name="modelBuilder">
    /// Model builder
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        /* TODO: configure schema */

        modelBuilder.ApplyConfiguration(new DomainEventReferenceConfiguration());
    }
}
