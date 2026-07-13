using System.Collections.Concurrent;
using System.Threading.Channels;
using Lewee.Application.Mediation.Notifications;

namespace Lewee.Infrastructure.ServerEvents;

/// <summary>
/// Server-Sent Events (SSE) Connection Manager
/// </summary>
/// <remarks>
/// Maintains a mapping of user IDs to their respective channels for order placements.
/// This allows for efficient management of SSE connections and ensures that each user has a dedicated channel
/// for receiving <see cref="ClientEvent" /> events.
/// </remarks>
internal sealed class ConnectionManager
{
    private readonly ConcurrentDictionary<string, Channel<ClientEvent>> userChannels
        = new(StringComparer.InvariantCultureIgnoreCase);

    /// <summary>
    /// Gets an existing channel or creates a new channel for the specified user to enable sending and receiving client
    /// events.
    /// </summary>
    /// <remarks>If a channel for the specified user already exists, it is returned; otherwise, a new
    /// unbounded channel is created. Channels are used to facilitate communication of client events on a per-user
    /// basis.</remarks>
    /// <param name="userId">The unique identifier of the user for whom the channel is being retrieved or created. This parameter cannot be
    /// null.</param>
    /// <returns>A channel that can be used to send and receive client events for the specified user.</returns>
    public Channel<ClientEvent> GetOrCreateChannel(string userId)
    {
        return this.userChannels.GetOrAdd(userId, _ => Channel.CreateUnbounded<ClientEvent>());
    }

    /// <summary>
    /// Gets the channel reader that streams client events for the specified user.
    /// </summary>
    /// <remarks>Use the returned channel reader to asynchronously receive events for the specified user. If
    /// the user does not have an associated channel, this method returns <see langword="null"/>.</remarks>
    /// <param name="userId">The unique identifier of the user whose channel reader is to be retrieved. Cannot be null or empty.</param>
    /// <returns>A <see cref="ChannelReader{ClientEvent}"/> instance for the specified user if one exists; otherwise, <see
    /// langword="null"/>.</returns>
    public ChannelReader<ClientEvent>? GetChannelReader(string userId)
    {
        return this.userChannels.TryGetValue(userId, out var channel) ? channel.Reader : null;
    }

    /// <summary>
    /// Attempts to retrieve the channel writer associated with the specified user identifier.
    /// </summary>
    /// <remarks>Use this method to safely access a user's channel writer without throwing an exception if the
    /// user does not exist. This is useful when the presence of a channel is uncertain.</remarks>
    /// <param name="userId">The unique identifier of the user whose channel writer is to be retrieved. Cannot be null.</param>
    /// <param name="writer">When this method returns, contains the channel writer for the specified user if found; otherwise, null. This
    /// parameter is passed uninitialized.</param>
    /// <returns>true if the channel writer was found and assigned to the out parameter; otherwise, false.</returns>
    public bool TryGetChannelWriter(string userId, out ChannelWriter<ClientEvent>? writer)
    {
        if (this.userChannels.TryGetValue(userId, out var channel))
        {
            writer = channel.Writer;
            return true;
        }

        writer = null;
        return false;
    }

    /// <summary>
    /// Gets the channel writers for all currently connected users.
    /// </summary>
    /// <remarks>Use the returned writers to broadcast a <see cref="ClientEvent"/> to every connected user.
    /// The snapshot is taken at the time of the call; channels added or removed concurrently may or may not be
    /// included.</remarks>
    /// <returns>An enumerable of <see cref="ChannelWriter{ClientEvent}"/> for every registered user channel.</returns>
    public IEnumerable<ChannelWriter<ClientEvent>> GetAllChannelWriters()
    {
        return this.userChannels.Values.Select(c => c.Writer);
    }

    /// <summary>
    /// Removes the user channel associated with the specified user identifier from the collection.
    /// </summary>
    /// <remarks>If the specified user identifier does not exist in the collection, no action is
    /// taken.</remarks>
    /// <param name="userId">The unique identifier of the user whose channel is to be removed. This parameter cannot be null.</param>
    public void RemoveChannel(string userId)
    {
        _ = this.userChannels.TryRemove(userId, out _);
    }
}
