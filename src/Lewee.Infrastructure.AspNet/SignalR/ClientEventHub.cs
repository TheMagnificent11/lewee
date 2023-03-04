using Lewee.Contracts;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace Lewee.Infrastructure.AspNet.SignalR;

/// <summary>
/// TClient Event Hub
/// </summary>
public class ClientEventHub : Hub
{
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientEventHub"/> class
    /// </summary>
    /// <param name="logger">Logger</param>
    public ClientEventHub(ILogger logger)
    {
        this.logger = logger.ForContext<ClientEventHub>();
    }

    /// <summary>
    /// Publish message to client
    /// </summary>
    /// <param name="clientId">Client ID</param>
    /// <param name="clientMessage">Client message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Asynchronous task</returns>
    public async Task Publish(string clientId, ClientMessage clientMessage, CancellationToken cancellationToken)
    {
        var client = this.Clients?.Client(clientId);
        if (client == null)
        {
            this.logger.Information("Could not find SignalR client {ClientId}", clientId);
            return;
        }

        await client.SendAsync(nameof(ClientMessage), clientMessage, cancellationToken);
        this.logger.Information("Published message to client");
    }
}
