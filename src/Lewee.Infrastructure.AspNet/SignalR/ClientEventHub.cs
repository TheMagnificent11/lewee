using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace Lewee.Infrastructure.AspNet.SignalR;

/// <summary>
/// Client Event Hub
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
        this.logger = logger;
    }

    /// <inheritdoc />
    public override async Task OnConnectedAsync()
    {
        this.logger.Debug("Client connected");

        var userId = this.Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrWhiteSpace(userId))
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, userId);
            this.logger.Debug("Client added to SignalR group");
        }

        await base.OnConnectedAsync();
    }
}
