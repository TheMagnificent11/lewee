using Microsoft.AspNetCore.Components;

namespace Lewee.Infrastructure.Fluxor;

/// <summary>
/// Service that holds the access token and persists it from server to WebAssembly client
/// using <see cref="PersistentComponentState"/>.
/// </summary>
/// <remarks>
/// <para>
/// This is the WebAssembly client-side implementation that receives the persisted access token.
/// The server-side implementation should populate the token from <c>HttpContext.GetTokenAsync("access_token")</c>.
/// </para>
/// <para>
/// Register this service on both server and client with <c>RegisterPersistentService</c> to enable
/// automatic state persistence across the prerendering boundary.
/// </para>
/// </remarks>
public class AccessTokenService
{
    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    [PersistentState]
    public virtual string? AccessToken { get; set; }
}
