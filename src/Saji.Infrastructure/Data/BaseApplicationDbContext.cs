using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Saji.Domain;
using Serilog;

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
    private readonly IMediator mediator;
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseApplicationDbContext{T}"/> class
    /// </summary>
    /// <param name="options">
    /// Database context options
    /// </param>
    /// <param name="mediator">Mediator</param>
    /// <param name="logger">Logger</param>
    protected BaseApplicationDbContext(DbContextOptions<T> options, IMediator mediator, ILogger logger)
        : base(options)
    {
        this.mediator = mediator;
        this.logger = logger.ForContext<BaseApplicationDbContext<T>>();
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
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await this.OnBeforeSaving(cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
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

    private async Task OnBeforeSaving(CancellationToken cancellationToken)
    {
        foreach (var entry in this.ChangeTracker.Entries().ToList())
        {
            await this.StoreAndDispatchDomainEvents(entry, cancellationToken);
        }
    }

    private async Task StoreAndDispatchDomainEvents(EntityEntry entry, CancellationToken cancellationToken)
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

            try
            {
                await this.mediator.Publish(domainEvent, cancellationToken);

                reference.Dispatch();

                this.logger.Information("Domain event dispatched {@DomainEvent}", domainEvent);
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Failed to dispatch domain event {@DomainEvent}", domainEvent);
            }

            this.DomainEventReferences?.Add(reference);
        }
    }
}
