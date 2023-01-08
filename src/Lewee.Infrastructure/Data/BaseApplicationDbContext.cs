using Lewee.Application.Auth;
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
    private readonly IAuthenticatedUserService authenticatedUserService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseApplicationDbContext{T}"/> class
    /// </summary>
    /// <param name="options">
    /// Database context options
    /// </param>
    /// <param name="authenticatedUserService">
    /// Authenticated user service
    /// </param>
    protected BaseApplicationDbContext(
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

    /// <inheritdoc />
    public DbSet<DomainEventReference>? DomainEventReferences { get; protected set; }

    /// <inheritdoc />
    public IQueryable<T> AggregateRoot<T>()
        where T : class, IAggregateRoot
    {
        return this.Set<T>();
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(this.Schema);

        modelBuilder.ApplyConfiguration(new DomainEventReferenceConfiguration());

        this.ConfigureDatabaseModel(modelBuilder);
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(new AuditDetailsSaveChangesInterceptor(this.authenticatedUserService));
        optionsBuilder.AddInterceptors(new DomainEventSaveChangesInterceptor<TContext>());
    }

    /// <summary>
    /// Conigures the database model
    /// </summary>
    /// <param name="modelBuilder">
    /// Database model builder
    /// </param>
    protected abstract void ConfigureDatabaseModel(ModelBuilder modelBuilder);
}
