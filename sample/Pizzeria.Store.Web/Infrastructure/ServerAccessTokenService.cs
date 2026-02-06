using System.Diagnostics.CodeAnalysis;
using Lewee.Infrastructure.Fluxor;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;

namespace Pizzeria.Store.Web.Infrastructure;

/// <summary>
/// Server-side access token service that retrieves the token from HttpContext
/// and persists it to WebAssembly via <see cref="PersistentComponentState"/>.
/// </summary>
internal sealed class ServerAccessTokenService : AccessTokenService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private string? cachedAccessToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerAccessTokenService"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public ServerAccessTokenService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets or sets the access token, retrieving it from HttpContext if not cached.
    /// </summary>
    [SuppressMessage(
        "Usage",
        "VSTHRD002:Avoid problematic synchronous waits",
        Justification = "Needed as this is an overriden propety getter")]
    public override string? AccessToken
    {
        get
        {
            if (this.cachedAccessToken is not null)
            {
                return this.cachedAccessToken;
            }

            var httpContext = this.httpContextAccessor.HttpContext;
            if (httpContext is null)
            {
                return null;
            }

            this.cachedAccessToken = httpContext.GetTokenAsync("access_token").GetAwaiter().GetResult();
            return this.cachedAccessToken;
        }

        set => this.cachedAccessToken = value;
    }
}
