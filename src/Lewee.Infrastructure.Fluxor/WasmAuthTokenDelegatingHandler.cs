using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.Fluxor;

/// <summary>
/// Delegating handler that adds the user's access token to outgoing HTTP requests in WebAssembly.
/// </summary>
public sealed class WasmAuthTokenDelegatingHandler : DelegatingHandler
{
    private readonly IAccessTokenProvider accessTokenProvider;
    private readonly ILogger<WasmAuthTokenDelegatingHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WasmAuthTokenDelegatingHandler"/> class.
    /// </summary>
    /// <param name="accessTokenProvider">Access token provider</param>
    /// <param name="logger">Logger</param>
    public WasmAuthTokenDelegatingHandler(
        IAccessTokenProvider accessTokenProvider,
        ILogger<WasmAuthTokenDelegatingHandler> logger)
    {
        this.accessTokenProvider = accessTokenProvider;
        this.logger = logger;
    }

    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tokenResult = await this.accessTokenProvider.RequestAccessToken();

        if (tokenResult.TryGetToken(out var token))
        {
            this.logger.LogDebug("Adding Bearer token to request: {RequestUri}", request.RequestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
        }
        else
        {
            this.logger.LogWarning(
                "Failed to get access token for request: {RequestUri}",
                request.RequestUri);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
