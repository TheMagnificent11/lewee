using Lewee.Shared;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Serilog.Context;

namespace Lewee.Infrastructure.AspNet.SignalR;

/// <summary>
/// Client Event Hub
/// </summary>
public class ClientEventHub : Hub
{
    private readonly ClientConnectionMapper clientConnectionMapper;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientEventHub"/> class
    /// </summary>
    /// <param name="clientConnectionMapper">Client connection mapper</param>
    /// <param name="httpContextAccessor">HTTP context accessor</param>
    /// <param name="logger">Logger</param>
    public ClientEventHub(
        ClientConnectionMapper clientConnectionMapper,
        IHttpContextAccessor httpContextAccessor,
        ILogger logger)
    {
        this.clientConnectionMapper = clientConnectionMapper;
        this.httpContextAccessor = httpContextAccessor;
        this.logger = logger;
    }

    /// <inheritdoc />
    public override async Task OnConnectedAsync()
    {
        // TODO: fix using this: https://consultwithgriff.com/signalr-connection-ids/
		using (LogContext.PushProperty("SignalRConnectionId", this.Context.ConnectionId))
        {
            this.logger.Debug("Client connected");

            if (this.httpContextAccessor?.HttpContext != null)
            {
                var connection = this.httpContextAccessor.HttpContext.Features.Get<IConnectionIdFeature>();
                if (connection != null)
                {
                    using (LogContext.PushProperty(LoggingConsts.ClientId, connection.ConnectionId))
                    {
                        this.clientConnectionMapper.Register(connection.ConnectionId, this.Context.ConnectionId);
                        this.logger.Debug("SignalR connection ID associted with client ID");
                    }
                }
            }

            await base.OnConnectedAsync();
        }
    }
}
