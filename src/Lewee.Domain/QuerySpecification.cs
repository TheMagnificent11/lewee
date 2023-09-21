using Ardalis.Specification;

namespace Lewee.Domain;

/// <summary>
/// Query Specification
/// </summary>
/// <typeparam name="T">Aggregate root type</typeparam>
public class QuerySpecification<T> : Specification<T>
    where T : AggregateRoot
{
}
