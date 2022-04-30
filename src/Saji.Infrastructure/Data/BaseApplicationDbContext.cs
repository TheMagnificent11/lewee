using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Saji.Domain;

namespace Saji.Infrastructure.Data;

/// <summary>
/// Base Appliation Database Context
/// </summary>
/// <typeparam name="T">
/// The type of this database context
/// </typeparam>
public abstract class BaseApplicationDbContext<T> : DbContext
    where T : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseApplicationDbContext{T}"/> class
    /// </summary>
    /// <param name="options">
    /// Database context options
    /// </param>
    protected BaseApplicationDbContext(DbContextOptions<T> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the database schema for the context
    /// </summary>
    public virtual string Schema { get; } = "dbo";

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

        /* TODO: ensure plurals used for table names */

        /* TODO: configure schema */

        modelBuilder.ApplyConfiguration(new DomainEventReferenceConfiguration());

        this.ConfigureDatabaseModel(modelBuilder);
    }

    /// <summary>
    /// Conigures the database model
    /// </summary>
    /// <param name="modelBuilder">
    /// Database model builder
    /// </param>
    protected abstract void ConfigureDatabaseModel(ModelBuilder modelBuilder);

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
