using Lewee.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Lewee.Infrastructure.AspNet.SignalR;

/// <summary>
/// TODO: Client Event Hub
/// </summary>
public class ClientEventHub : Hub
{
    /// <summary>
    /// Publish message to client
    /// </summary>
    /// <param name="clientId">Client ID</param>
    /// <param name="clientMessage">Client message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Asynchronous task</returns>
    public async Task Publish(string clientId, ClientMessge clientMessage, CancellationToken cancellationToken)
    {
        await this.Clients.Clients(clientId).SendAsync(
            nameof(ClientMessge),
            clientMessage,
            cancellationToken);
    }
}
