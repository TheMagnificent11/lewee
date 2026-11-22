using Lewee.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Base Repository
/// </summary>
/// <typeparam name="TAggregate">Aggregate root type</typeparam>
/// <typeparam name="TContext">Database context type</typeparam>
public class Repository<TAggregate, TContext> : IRepository<TAggregate>
    where TAggregate : AggregateRoot
    where TContext : DbContext
{
    private readonly TContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{TAggregate, TContext}"/> class
    /// </summary>
    /// <param name="context">Database context</param>
    public Repository(TContext context)
    {
        this.context = context;
    }

    /// <inheritdoc />
    public async Task<List<TAggregate>> AllAsync(CancellationToken cancellationToken = default)
    {
        return await this.context.Set<TAggregate>().ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<TAggregate>> QueryAsync(
        QuerySpecification<TAggregate> querySpecification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(querySpecification);

        var query = this.ApplySpecification(querySpecification);
        return await query.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TAggregate?> QueryOneAsync(
        QuerySpecification<TAggregate> querySpecification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(querySpecification);

        var query = this.ApplySpecification(querySpecification);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TAggregate?> RetrieveByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await this.context.Set<TAggregate>().FindAsync([id], cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(TAggregate entity, CancellationToken cancellationToken = default)
    {
        await this.context.Set<TAggregate>().AddAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await this.context.SaveChangesAsync(cancellationToken);
    }

    private static string GetPropertyName(System.Linq.Expressions.LambdaExpression expression)
    {
        // Extract property name from expression
        if (expression.Body is System.Linq.Expressions.MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }

        throw new InvalidOperationException("Unable to extract property name from expression");
    }

    private IQueryable<TAggregate> ApplySpecification(QuerySpecification<TAggregate> specification)
    {
        var query = this.context.Set<TAggregate>().AsQueryable();

        // Apply where clauses
        foreach (var whereExpression in specification.WhereExpressions)
        {
            query = query.Where(whereExpression);
        }

        // Apply includes - need to build the chain properly
        var includeChain = new List<string>();
        foreach (var includeExpression in specification.IncludeExpressions)
        {
            if (includeExpression.IsThenInclude)
            {
                // Append to the current chain
                var propertyName = GetPropertyName(includeExpression.Expression);
                if (includeChain.Count > 0)
                {
                    includeChain[^1] += "." + propertyName;
                }
            }
            else
            {
                // Start a new chain
                var propertyName = GetPropertyName(includeExpression.Expression);
                includeChain.Add(propertyName);
            }
        }

        // Apply all include chains
        foreach (var includePath in includeChain)
        {
            query = query.Include(includePath);
        }

        return query;
    }
}
