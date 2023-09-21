using Ardalis.Specification.EntityFrameworkCore;
using Lewee.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Base Repository
/// </summary>
/// <typeparam name="TAggregate">Aggregate root type</typeparam>
/// <typeparam name="TContext">Database context type</typeparam>
public class Repository<TAggregate, TContext> : RepositoryBase<TAggregate>, IRepository<TAggregate>
    where TAggregate : AggregateRoot
    where TContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{TAggregate, TContext}"/> class
    /// </summary>
    /// <param name="dbContextFactory">Database context factory</param>
    public Repository(IDbContextFactory<TContext> dbContextFactory)
        : base(dbContextFactory.CreateDbContext())
    {
    }

    /// <inheritdoc />
    public async Task<List<TAggregate>> All(CancellationToken cancellationToken = default)
    {
        return await this.ListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<TAggregate>> Query(
        QuerySpecification<TAggregate> querySpecification,
        CancellationToken cancellationToken = default)
    {
        return await this.ListAsync(querySpecification, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TAggregate?> QueryOne(
        QuerySpecification<TAggregate> querySpecification,
        CancellationToken cancellationToken = default)
    {
        return await this.FirstOrDefaultAsync(querySpecification, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TAggregate?> RetrieveById(Guid id, CancellationToken cancellationToken = default)
    {
        return await this.GetByIdAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public void Add(TAggregate entity)
    {
        this.AddAsync(entity);
    }

    /// <inheritdoc />
    public async Task<int> SaveChanges(CancellationToken cancellationToken = default)
    {
        return await this.SaveChangesAsync(cancellationToken);
    }
}
