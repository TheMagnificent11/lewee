using Lewee.Domain;

namespace Lewee.Application.Mediation.Requests;

/// <summary>
/// Tenant Aggregate Root Query
/// </summary>
/// <typeparam name="TAggregate">Aggregate root type</typeparam>
/// <typeparam name="TDto">DTO result type</typeparam>
public class TenantAggregateRootQuery<TAggregate, TDto> : AggregateRootQuery<TAggregate, TDto>, ITenantRequest
    where TAggregate : class, IAggregateRoot
    where TDto : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TenantAggregateRootQuery{TAggregate, TDto}"/> class
    /// </summary>
    /// <param name="correlationId">Correlation ID</param>
    /// <param name="tenantId">Tenant ID</param>
    public TenantAggregateRootQuery(Guid correlationId, Guid tenantId)
        : base(correlationId)
    {
        this.TenantId = tenantId;
    }

    /// <inheritdoc />
    public Guid TenantId { get; }
}
