using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.AspNet.SignalR;

/// <summary>
/// Client Event Hub
/// </summary>
[AllowAnonymous]
public class ClientEventHub : Hub
{
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientEventHub"/> class
    /// </summary>
    /// <param name="logger">Logger</param>
    public ClientEventHub(ILogger<ClientEventHub> logger)
    {
        this.logger = logger;
    }

    /// <inheritdoc />
    public override async Task OnConnectedAsync()
    {
        this.logger.LogDebug("Client connected");

        var userId = this.Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrWhiteSpace(userId))
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, userId);
            this.logger.LogDebug("Client added to SignalR group for user {UserId}", userId);
        }
        else
        {
            this.logger.LogDebug("Anonymous client connected to SignalR hub");
        }

        await base.OnConnectedAsync();
    }
}
