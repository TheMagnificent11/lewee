using Microsoft.EntityFrameworkCore;

namespace Saji.Infrastructure.Data;

/// <summary>
/// Base Appliation Database Context
/// </summary>
public abstract class BaseApplicationDbContext : DbContext
{
    /// <summary>
    /// Gets or sets the database schema for the context
    /// </summary>
    public virtual string Schema { get; protected set; } = "dbo";

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
