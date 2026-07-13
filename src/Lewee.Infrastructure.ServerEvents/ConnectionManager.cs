using System.Collections.Concurrent;
using System.Threading.Channels;
using Lewee.Application.Mediation.Notifications;

namespace Lewee.Infrastructure.ServerEvents;

/// <summary>
/// Server-Sent Events (SSE) Connection Manager
/// </summary>
/// <remarks>
/// Maintains a mapping of connection IDs to their respective channels.
/// Each SSE connection gets its own dedicated channel, so multiple tabs or circuits for the same user
/// do not compete for messages and a single disconnect does not affect other active connections.
/// </remarks>
internal sealed class ConnectionManager
{
    private readonly ConcurrentDictionary<string, (string UserId, Channel<ClientEvent> Channel)> connectionChannels
        = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets an existing channel or creates a new channel for the specified connection to enable sending and receiving
    /// client events.
    /// </summary>
    /// <remarks>If a channel for the specified connection already exists, it is returned; otherwise, a new
    /// unbounded channel is created. Each connection has its own dedicated channel to avoid competing-consumer
    /// issues when the same user has multiple active connections.</remarks>
    /// <param name="userId">The unique identifier of the user associated with this connection.</param>
    /// <param name="connectionId">The unique identifier of the connection for which the channel is being retrieved or created.</param>
    /// <returns>A channel that can be used to send and receive client events for the specified connection.</returns>
    public Channel<ClientEvent> GetOrCreateChannel(string userId, string connectionId)
    {
        return this.connectionChannels
            .GetOrAdd(connectionId, _ => (userId, Channel.CreateUnbounded<ClientEvent>()))
            .Channel;
    }

    /// <summary>
    /// Gets the channel writers for all connections belonging to the specified user.
    /// </summary>
    /// <remarks>Use the returned writers to send a <see cref="ClientEvent"/> to every active connection for a
    /// particular user. The snapshot is taken at the time of the call; connections added or removed concurrently
    /// may or may not be included.</remarks>
    /// <param name="userId">The unique identifier of the user whose connection writers are to be retrieved.</param>
    /// <returns>An enumerable of <see cref="ChannelWriter{ClientEvent}"/> for every active connection belonging to
    /// the specified user.</returns>
    public IEnumerable<ChannelWriter<ClientEvent>> GetChannelWritersForUser(string userId)
    {
        return this.connectionChannels.Values
            .Where(v => string.Equals(v.UserId, userId, StringComparison.OrdinalIgnoreCase))
            .Select(v => v.Channel.Writer);
    }

    /// <summary>
    /// Gets the channel writers for all currently active connections.
    /// </summary>
    /// <remarks>Use the returned writers to broadcast a <see cref="ClientEvent"/> to every connected client.
    /// The snapshot is taken at the time of the call; connections added or removed concurrently may or may not be
    /// included.</remarks>
    /// <returns>An enumerable of <see cref="ChannelWriter{ClientEvent}"/> for every active connection.</returns>
    public IEnumerable<ChannelWriter<ClientEvent>> GetAllChannelWriters()
    {
        return this.connectionChannels.Values.Select(v => v.Channel.Writer);
    }

    /// <summary>
    /// Removes the channel associated with the specified connection identifier from the collection.
    /// </summary>
    /// <remarks>If the specified connection identifier does not exist in the collection, no action is
    /// taken.</remarks>
    /// <param name="connectionId">The unique identifier of the connection whose channel is to be removed.</param>
    public void RemoveChannel(string connectionId)
    {
        if (this.connectionChannels.TryRemove(connectionId, out var entry))
        {
            entry.Channel.Writer.TryComplete();
        }
    }
}
