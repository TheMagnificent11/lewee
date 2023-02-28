using Lewee.Blazor.Fluxor.Actions;

namespace Lewee.Blazor.Events;

/// <summary>
/// Message To Action Mapper Interface
/// </summary>
public interface IMessageToActionMapper
{
    /// <summary>
    /// Maps a message to <see cref="IMessageReceivedAction"/> Fluxor action
    /// </summary>
    /// <param name="message">Message to map</param>
    /// <returns>Mapped Fluxor action</returns>
    IMessageReceivedAction? Map(object message);
}
