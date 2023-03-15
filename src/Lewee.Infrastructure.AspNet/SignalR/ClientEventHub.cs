using Lewee.Shared;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Serilog.Context;

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
        using (LogContext.PushProperty(LoggingConsts.ClientId, this.Context.ConnectionId))
        {
            this.logger.Debug("Client connected");
            await base.OnConnectedAsync();
        }
    }
}
