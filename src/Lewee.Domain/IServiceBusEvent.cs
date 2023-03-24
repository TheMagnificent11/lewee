using MediatR;

namespace Lewee.Domain;

/// <summary>
/// Service Bus Event Interface
/// </summary>
/// <remarks>
/// Add this interface to any implementation of <see cref="IDomainEvent"/> in order to publish it to a service bus
/// </remarks>
public interface IServiceBusEvent : INotification
{
    /// <summary>
    /// Gets the correlation ID
    /// </summary>
    Guid CorrelationId { get; }

    /// <summary>
    /// Converts the <see cref="IDomainEvent"/> with <see cref="IServiceBusEvent"/> marker to the desired service bus message
    /// </summary>
    /// <returns>Service bus message</returns>
    object ToMessage();
}
