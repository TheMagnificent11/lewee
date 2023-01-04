using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Saji.Domain;
using Saji.Infrastructure.Events;

namespace Saji.Infrastructure.Data;

/// <summary>
/// Base Appliation Database Context
/// </summary>
/// <typeparam name="TContext">
/// The type of this database context
/// </typeparam>
public abstract class BaseApplicationDbContext<TContext> : DbContext, IDbContext
    where TContext : DbContext, IDbContext
{
    private readonly DomainEventDispatcher<TContext> domainEventDispatcher;

    private bool domainEventsAdded = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseApplicationDbContext{T}"/> class
    /// </summary>
    /// <param name="options">
    /// Database context options
    /// </param>
    /// <param name="domainEventDispatcher">Domain event dispatcher</param>
    protected BaseApplicationDbContext(DbContextOptions<TContext> options, DomainEventDispatcher<TContext> domainEventDispatcher)
        : base(options)
    {
        this.domainEventDispatcher = domainEventDispatcher;
    }

    /// <summary>
    /// Gets the database schema for the context
    /// </summary>
    public virtual string Schema { get; } = "dbo";

    /// <summary>
    /// Gets or sets the domain event referecnce database set
    /// </summary>
    public DbSet<DomainEventReference>? DomainEventReferences { get; protected set; }

    /// <summary>
    /// Returns a queryable collection for an <see cref="IAggregateRoot"/> entity type
    /// </summary>
    /// <typeparam name="T">Aggregate root type</typeparam>
    /// <returns>A ueryable collection for an <see cref="IAggregateRoot"/> entity type</returns>
    public IQueryable<T> AggregateRoot<T>()
        where T : class, IAggregateRoot
    {
        return this.Set<T>();
    }

    /// <summary>
    /// Saves all changes made in this context to the database
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// An async task that contains the number of changes that were persisted to the database
    /// </returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.OnBeforeSaving();

        var changes = await base.SaveChangesAsync(cancellationToken);

        if (this.domainEventsAdded)
        {
            await this.domainEventDispatcher.DispatchEvents(cancellationToken);
        }

        return changes;
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

        modelBuilder.HasDefaultSchema(this.Schema);

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
        this.domainEventsAdded = false;

        foreach (var entry in this.ChangeTracker.Entries().ToList())
        {
            this.StoreAndDispatchDomainEvents(entry);
        }
    }

    private void StoreAndDispatchDomainEvents(EntityEntry entry)
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

            this.DomainEventReferences?.Add(reference);

            this.domainEventsAdded = true;
        }
    }
}
