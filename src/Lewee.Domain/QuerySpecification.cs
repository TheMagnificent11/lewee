using System.Linq.Expressions;

namespace Lewee.Domain;

/// <summary>
/// Query Specification
/// </summary>
/// <typeparam name="T">Aggregate root type</typeparam>
public abstract class QuerySpecification<T>
    where T : AggregateRoot
{
    private readonly List<Expression<Func<T, bool>>> whereExpressions = [];
    private readonly List<IncludeExpression> includeExpressions = [];

    /// <summary>
    /// Query builder interface for fluent specification configuration
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    protected interface IQueryBuilder<TEntity>
        where TEntity : AggregateRoot
    {
        /// <summary>
        /// Adds a where clause to filter entities
        /// </summary>
        /// <param name="predicate">Filter predicate</param>
        /// <returns>The query builder for chaining</returns>
        IIncludableQueryBuilder<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Adds an include for eager loading of a navigation property
        /// </summary>
        /// <typeparam name="TProperty">Property type</typeparam>
        /// <param name="navigationProperty">Navigation property expression</param>
        /// <returns>The includable query builder for chaining</returns>
        IIncludableQueryBuilder<TEntity> Include<TProperty>(
            Expression<Func<TEntity, TProperty>> navigationProperty);
    }

    /// <summary>
    /// Includable query builder interface for fluent specification configuration
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    protected interface IIncludableQueryBuilder<TEntity>
        where TEntity : AggregateRoot
    {
        /// <summary>
        /// Adds a where clause to filter entities
        /// </summary>
        /// <param name="predicate">Filter predicate</param>
        /// <returns>The query builder for chaining</returns>
        IIncludableQueryBuilder<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Adds an include for eager loading of a navigation property
        /// </summary>
        /// <typeparam name="TProperty">Property type</typeparam>
        /// <param name="navigationProperty">Navigation property expression</param>
        /// <returns>The includable query builder for chaining</returns>
        IIncludableQueryBuilder<TEntity> Include<TProperty>(
            Expression<Func<TEntity, TProperty>> navigationProperty);

        /// <summary>
        /// Adds a then-include for eager loading of a nested navigation property
        /// </summary>
        /// <typeparam name="TPrevious">Previous property item type (for collections, this is the item type)</typeparam>
        /// <typeparam name="TProperty">Property type</typeparam>
        /// <param name="navigationProperty">Navigation property expression</param>
        /// <returns>The includable query builder for chaining</returns>
        IIncludableQueryBuilder<TEntity> ThenInclude<TPrevious, TProperty>(
            Expression<Func<TPrevious, TProperty>> navigationProperty);
    }

    /// <summary>
    /// Gets the where expressions for filtering
    /// </summary>
    public IReadOnlyList<Expression<Func<T, bool>>> WhereExpressions => this.whereExpressions.AsReadOnly();

    /// <summary>
    /// Gets the include expressions for eager loading
    /// </summary>
    public IReadOnlyList<IncludeExpression> IncludeExpressions => this.includeExpressions.AsReadOnly();

    /// <summary>
    /// Gets the query builder to configure the specification
    /// </summary>
    protected IQueryBuilder<T> Query => new QueryBuilder(this);

    /// <summary>
    /// Represents an include expression for eager loading
    /// </summary>
    public sealed class IncludeExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncludeExpression"/> class
        /// </summary>
        /// <param name="expression">The include expression</param>
        /// <param name="isThenInclude">Whether this is a ThenInclude</param>
        public IncludeExpression(LambdaExpression expression, bool isThenInclude = false)
        {
            this.Expression = expression;
            this.IsThenInclude = isThenInclude;
        }

        /// <summary>
        /// Gets the include expression
        /// </summary>
        public LambdaExpression Expression { get; }

        /// <summary>
        /// Gets a value indicating whether this is a ThenInclude
        /// </summary>
        public bool IsThenInclude { get; }
    }

    private sealed class QueryBuilder : IQueryBuilder<T>, IIncludableQueryBuilder<T>
    {
        private readonly QuerySpecification<T> specification;

        public QueryBuilder(QuerySpecification<T> specification)
        {
            this.specification = specification;
        }

        public IIncludableQueryBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            this.specification.whereExpressions.Add(predicate);
            return this;
        }

        public IIncludableQueryBuilder<T> Include<TProperty>(
            Expression<Func<T, TProperty>> navigationProperty)
        {
            this.specification.includeExpressions.Add(new IncludeExpression(navigationProperty));
            return this;
        }

        public IIncludableQueryBuilder<T> ThenInclude<TPrevious, TProperty>(
            Expression<Func<TPrevious, TProperty>> navigationProperty)
        {
            this.specification.includeExpressions.Add(new IncludeExpression(navigationProperty, isThenInclude: true));
            return this;
        }
    }
}
