using MediatR;

namespace Lewee.Domain;

/// <summary>
/// Domain Event Interface
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>
    /// Gets the correlation ID of the event
    /// </summary>
    Guid CorrelationId { get; }
}
