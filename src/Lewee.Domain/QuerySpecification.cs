using System.Linq.Expressions;

namespace Lewee.Domain;

/// <summary>
/// Query Specification
/// </summary>
/// <typeparam name="T">Aggregate root type</typeparam>
public class QuerySpecification<T>
    where T : AggregateRoot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuerySpecification{T}"/> class
    /// </summary>
    /// <param name="criteria">Query criteria</param>
    public QuerySpecification(Expression<Func<T, bool>> criteria)
    {
        this.Criteria = criteria;
        this.Includes = new();
    }

    /// <summary>
    /// Gets the query criteria
    /// </summary>
    public Expression<Func<T, bool>> Criteria { get; private set; }

    /// <summary>
    /// Gets the query includes
    /// </summary>
    public List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// Adds an include
    /// </summary>
    /// <param name="includeExpression">Include expression</param>
    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        this.Includes.Add(includeExpression);
    }
}
