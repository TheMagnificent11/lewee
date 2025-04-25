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
    /// <param name="context">Database context</param>
    public Repository(TContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<List<TAggregate>> AllAsync(CancellationToken cancellationToken = default)
    {
        return await this.ListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<TAggregate>> QueryAsync(
        QuerySpecification<TAggregate> querySpecification,
        CancellationToken cancellationToken = default)
    {
        return await this.ListAsync(querySpecification, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TAggregate?> QueryOneAsync(
        QuerySpecification<TAggregate> querySpecification,
        CancellationToken cancellationToken = default)
    {
        return await this.FirstOrDefaultAsync(querySpecification, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TAggregate?> RetrieveByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await this.GetByIdAsync(id, cancellationToken);
    }

    Task IRepository<TAggregate>.AddAsync(TAggregate entity, CancellationToken cancellationToken)
    {
        return base.AddAsync(entity, cancellationToken);
    }
}
