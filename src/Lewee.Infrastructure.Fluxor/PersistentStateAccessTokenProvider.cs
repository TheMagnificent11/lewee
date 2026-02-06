using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Lewee.Infrastructure.Fluxor;

/// <summary>
/// An <see cref="IAccessTokenProvider"/> implementation that uses the persisted access token
/// from the server via <see cref="AccessTokenService"/>.
/// </summary>
/// <remarks>
/// <para>
/// This provider is designed for Blazor Web Apps with WebAssembly interactivity where
/// authentication is handled on the server side (e.g., OpenID Connect with Keycloak).
/// </para>
/// <para>
/// The access token is persisted from the server during prerendering using
/// <see cref="Microsoft.AspNetCore.Components.PersistentComponentState"/> and made available
/// to WebAssembly components through this provider.
/// </para>
/// </remarks>
public sealed class PersistentStateAccessTokenProvider : IAccessTokenProvider
{
    private readonly AccessTokenService accessTokenService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistentStateAccessTokenProvider"/> class.
    /// </summary>
    /// <param name="accessTokenService">The access token service containing the persisted token.</param>
    public PersistentStateAccessTokenProvider(AccessTokenService accessTokenService)
    {
        this.accessTokenService = accessTokenService;
    }

    /// <inheritdoc/>
    public ValueTask<AccessTokenResult> RequestAccessToken()
    {
        return ValueTask.FromResult(this.GetAccessTokenResult());
    }

    /// <inheritdoc/>
    public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
    {
        // Options are ignored as we use server-side authentication
        return ValueTask.FromResult(this.GetAccessTokenResult());
    }

    private AccessTokenResult GetAccessTokenResult()
    {
        if (string.IsNullOrEmpty(this.accessTokenService.AccessToken))
        {
            return new AccessTokenResult(
                AccessTokenResultStatus.RequiresRedirect,
                new AccessToken(),
                interactiveRequestUrl: "/",
                interactiveRequest: null);
        }

        var accessToken = new AccessToken
        {
            Value = this.accessTokenService.AccessToken,
        };

        return new AccessTokenResult(
            AccessTokenResultStatus.Success,
            accessToken,
            interactiveRequestUrl: null,
            interactiveRequest: null);
    }
}
