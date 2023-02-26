using Microsoft.AspNetCore.Http;

namespace Lewee.Infrastructure.AspNet.SignalR;

/// <summary>
/// SignalR Client ID Middleware
/// </summary>
public class SignalRClientIdMiddleware
{
    /// <summary>
    /// <see cref="IHttpContextAccessor"/> item ID for storing the SignalR client ID
    /// </summary>
    public const string ClientIdHttpContextId = "SignalR-Client-Id";

    private const string SignalRClientIdHeder = "X-Client-Id";

    private readonly RequestDelegate next;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignalRClientIdMiddleware"/> class
    /// </summary>
    /// <param name="next">Next request delegate</param>
    public SignalRClientIdMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    /// <summary>
    /// Invokes the middleware
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>An asynchronous task</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(SignalRClientIdHeder, out var clientId)
            && !string.IsNullOrEmpty(clientId)
            && Guid.TryParse(clientId, out _))
        {
            context.Items[SignalRClientIdHeder] = clientId;
        }

        await this.next(context);
    }
}
