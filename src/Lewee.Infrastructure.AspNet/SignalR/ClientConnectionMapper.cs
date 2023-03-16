using System.Collections.Concurrent;

namespace Lewee.Infrastructure.AspNet.SignalR;

/// <summary>
/// Client Connection Mapper
/// </summary>
public class ClientConnectionMapper
{
    private readonly ConcurrentDictionary<string, List<string>> mappings = new();

    /// <summary>
    /// Assoicates a connection ID with a client ID
    /// </summary>
    /// <param name="clientId">Client ID</param>
    /// <param name="connectionId">Connection ID</param>
    public void Register(string clientId, string connectionId)
    {
        var connectionIds = this.mappings.GetOrAdd(clientId, new List<string>());

        if (connectionIds.Contains(connectionId))
        {
            return;
        }

        connectionIds.Add(connectionId);
    }

    /// <summary>
    /// Gets the connections IDs associated with a client ID
    /// </summary>
    /// <param name="clientId">Client ID</param>
    /// <returns>A list of connection IDs assoicated with the client ID (potentially empty)</returns>
    public List<string> GetConnections(string clientId)
    {
        if (this.mappings.TryGetValue(clientId, out var connections))
        {
            return connections;
        }

        return new List<string>();
    }
}
