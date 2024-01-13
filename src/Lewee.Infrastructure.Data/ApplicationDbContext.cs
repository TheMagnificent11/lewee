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
    private readonly IAuthenticatedUserService authenticatedUserService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext{T}"/> class
    /// </summary>
    /// <param name="options">
    /// Database context options
    /// </param>
    /// <param name="authenticatedUserService">
    /// Authenticated user service
    /// </param>
    protected ApplicationDbContext(
        DbContextOptions<TContext> options,
        IAuthenticatedUserService authenticatedUserService)
        : base(options)
    {
        this.authenticatedUserService = authenticatedUserService;
    }

    /// <summary>
    /// Gets the database schema for the context
    /// </summary>
    public virtual string Schema { get; } = "dbo";

    /// <inheritdoc/>
    public DbSet<DomainEventReference>? DomainEventReferences { get; internal set; }

    /// <inheritdoc/>
    public DbSet<QueryProjectionReference>? QueryProjectionReferences { get; internal set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(this.Schema);

        modelBuilder.ApplyConfiguration(new DomainEventReferenceConfiguration());
        modelBuilder.ApplyConfiguration(new QueryProjectionReferenceConfiguration());

        this.ConfigureDatabaseModel(modelBuilder);
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(new AuditDetailsSaveChangesInterceptor(this.authenticatedUserService));
        optionsBuilder.AddInterceptors(new DomainEventSaveChangesInterceptor<TContext>(this.authenticatedUserService));
    }

    /// <summary>
    /// Configures the database model
    /// </summary>
    /// <param name="modelBuilder">
    /// Database model builder
    /// </param>
    protected abstract void ConfigureDatabaseModel(ModelBuilder modelBuilder);
}
