using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Lewee.Common;
using Microsoft.AspNetCore.Components.Authorization;

namespace Lewee.Infrastructure.Fluxor;

/// <summary>
/// WebAssembly implementation of <see cref="IAuthenticatedUserService"/> that retrieves
/// the user ID from <see cref="AuthenticationStateProvider"/>.
/// </summary>
public sealed class WasmAuthenticatedUserService : IAuthenticatedUserService
{
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private bool isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="WasmAuthenticatedUserService"/> class.
    /// </summary>
    /// <param name="authenticationStateProvider">The authentication state provider.</param>
    public WasmAuthenticatedUserService(AuthenticationStateProvider authenticationStateProvider)
    {
        this.authenticationStateProvider = authenticationStateProvider;
    }

    /// <inheritdoc/>
    [SuppressMessage(
        "Usage",
        "VSTHRD002:Avoid problematic synchronous waits",
        Justification = "Required for property getter; in WebAssembly this typically completes synchronously after initial load")]
    public string? UserId
    {
        get
        {
            if (this.isInitialized)
            {
                return field;
            }

            var authState = this.authenticationStateProvider.GetAuthenticationStateAsync()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            field = authState.User.FindFirstValue(ClaimTypes.NameIdentifier);
            this.isInitialized = true;

            return field;
        }
    }
}
