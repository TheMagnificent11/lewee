using System.Diagnostics.CodeAnalysis;

namespace Lewee.Infrastructure.Fluxor;

/// <summary>
/// Message to Action Mapper Interface
/// </summary>
public interface IMessageToActionMapper
{
    /// <summary>
    /// Maps a message to a Fluxor action
    /// </summary>
    /// <param name="message">Message to map</param>
    /// <param name="correlationId">Correlation ID</param>
    /// <returns>Fluxor action or null if no mapping exists</returns>
    IMessageReceivedAction? Map([NotNull] object message, Guid correlationId);
}
