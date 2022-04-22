using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Saji.Domain;

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

    /* TODO: make non-nullable */

    /// <summary>
    /// Gets or sets the domain event referecnce database set
    /// </summary>
    public DbSet<DomainEventReference>? DomainEventReferences { get; protected set; }

    /// <summary>
    /// Saves all changes made in this context to the database
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// An async task that contains the number of changes that were persisted to the database
    /// </returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.OnBeforeSaving();

        return base.SaveChangesAsync(cancellationToken);
    }

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

    private void OnBeforeSaving()
    {
        foreach (var entry in this.ChangeTracker.Entries().ToList())
        {
            this.StoredDomainEvents(entry);
        }
    }

    private void StoredDomainEvents(EntityEntry entry)
    {
        if (entry.Entity is not BaseEntity baseEntity)
        {
            return;
        }

        var events = baseEntity.DomainEvents.GetAndClear();

        foreach (var domainEvent in events)
        {
            if (domainEvent == null)
            {
                continue;
            }

            var reference = new DomainEventReference(domainEvent);

            this.Add(reference);
        }
    }
}
