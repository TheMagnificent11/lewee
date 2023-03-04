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

    private const string SignalXUserAgentHeader = "x-signalr-user-agent";
    private const string SignalRVersion = "7.0.3";

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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var connectionId = context.Request.Headers[SignalXUserAgentHeader]
            .FirstOrDefault(x => x.StartsWith("Microsoft SignalR/", StringComparison.OrdinalIgnoreCase));
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (!string.IsNullOrEmpty(connectionId))
        {
            /* Example header value:
             * Microsoft SignalR/7.0 (7.0.3+febee99db845fd8766a13bdb391a07c3ee90b4ba; Unknown OS; .NET; .NET 7.0.3)
             */

            var segments = connectionId.Split(new char[] { ';', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
            var idSegment = segments.FirstOrDefault(x => x.StartsWith(SignalRVersion));
            if (idSegment != null)
            {
                var idSegmentParts = idSegment.Split('+');
                if (idSegmentParts.Length >= 2)
                {
                    var id = idSegmentParts[1];
                    context.Items[ClientIdHttpContextId] = id;
                }
            }
        }

        await this.next(context);
    }
}
