using Lewee.Domain;
using MediatR;

namespace Lewee.Application.Mediation.Requests;

/// <summary>
/// Aggreate Root Query
/// </summary>
/// <typeparam name="TAggregate">Aggregate root type</typeparam>
/// <typeparam name="TDto">DTO result type</typeparam>
public class AggregateRootQuery<TAggregate, TDto>
    : IRequest<QueryResult<IEnumerable<TDto>>>, IQuery<IEnumerable<TDto>>
    where TAggregate : class, IAggregateRoot
    where TDto : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootQuery{TAggregate, TDto}"/> class
    /// </summary>
    /// <param name="correlationId">Correlation ID</param>
    public AggregateRootQuery(Guid correlationId)
    {
        this.CorrelationId = correlationId;
    }

    /// <inheritdoc />
    public Guid CorrelationId { get; }
}
