using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.Refit;

/// <summary>
/// Delegating handler that adds the user's access token to outgoing HTTP requests.
/// </summary>
internal sealed class AuthTokenDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ILogger<AuthTokenDelegatingHandler> logger;

    public AuthTokenDelegatingHandler(
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthTokenDelegatingHandler> logger)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var httpContext = this.httpContextAccessor.HttpContext;

        var accessToken = await httpContext!.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(accessToken))
        {
            this.logger.LogAccessTokenNullOrEmpty(request.RequestUri!);
        }
        else
        {
            this.logger.LogAddingBearerToken(request.RequestUri!);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
