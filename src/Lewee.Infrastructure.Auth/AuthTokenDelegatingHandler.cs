using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.Auth;

/// <summary>
/// Delegating handler that adds the user's access token to outgoing HTTP requests.
/// </summary>
public sealed class AuthTokenDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ILogger<AuthTokenDelegatingHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthTokenDelegatingHandler"/> class
    /// </summary>
    /// <param name="httpContextAccessor">HTTP context accessor</param>
    /// <param name="logger">Logger</param>
    public AuthTokenDelegatingHandler(
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthTokenDelegatingHandler> logger)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.logger = logger;
    }

    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var httpContext = this.httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            this.logger.LogWarning("HttpContext is null - cannot retrieve access token");
        }
        else
        {
            var accessToken = await httpContext.GetTokenAsync("access_token");
            if (!string.IsNullOrEmpty(accessToken))
            {
                this.logger.LogDebug("Adding Bearer token to request: {RequestUri}", request.RequestUri);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            else
            {
                this.logger.LogWarning("Access token is null or empty for request: {RequestUri}", request.RequestUri);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
