using Lewee.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Application Database Context
/// </summary>
/// <typeparam name="TContext">
/// The type of this database context
/// </typeparam>
public abstract class ApplicationDbContext<TContext> : DbContext, IApplicationDbContext
    where TContext : DbContext, IApplicationDbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext{T}"/> class
    /// </summary>
    /// <param name="options">
    /// Database context options
    /// </param>
    protected ApplicationDbContext(DbContextOptions<TContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the database schema for the context
    /// </summary>
    public virtual string? Schema { get; }

    /// <inheritdoc/>
    public DbSet<DomainEventReference>? DomainEventReferences { get; internal set; }

    /// <inheritdoc/>
    public DbSet<QueryProjectionReference>? QueryProjectionReferences { get; internal set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        if (!string.IsNullOrWhiteSpace(this.Schema))
        {
            modelBuilder.HasDefaultSchema(this.Schema);
        }

        modelBuilder.ApplyConfiguration(new DomainEventReferenceConfiguration());
        modelBuilder.ApplyConfiguration(new QueryProjectionReferenceConfiguration());

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TContext).Assembly);
    }
}
