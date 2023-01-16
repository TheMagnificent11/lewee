using Lewee.Domain;

namespace Lewee.Application.Mediation.Requests;

/// <summary>
/// Aggreate Root Query Interface
/// </summary>
/// <typeparam name="TAggregate">Aggregate root type</typeparam>
/// <typeparam name="TDto">DTO result type</typeparam>
public interface IAggregateRootQuery<TAggregate, TDto> : IQuery<IEnumerable<TDto>>
    where TAggregate : class, IAggregateRoot
    where TDto : class
{
}
