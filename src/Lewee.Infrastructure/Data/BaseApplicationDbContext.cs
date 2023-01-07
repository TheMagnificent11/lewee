using Lewee.Application.Data;
using Lewee.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Base Appliation Database Context
/// </summary>
/// <typeparam name="TContext">
/// The type of this database context
/// </typeparam>
public abstract class BaseApplicationDbContext<TContext> : DbContext, IDbContext
    where TContext : DbContext, IDbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseApplicationDbContext{T}"/> class
    /// </summary>
    /// <param name="options">
    /// Database context options
    /// </param>
    protected BaseApplicationDbContext(DbContextOptions<TContext> options)
        : base(options)
    {
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
    /// <returns>A queryable collection for an <see cref="IAggregateRoot"/> entity type</returns>
    public IQueryable<T> AggregateRoot<T>()
        where T : class, IAggregateRoot
    {
        return this.Set<T>();
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
}
