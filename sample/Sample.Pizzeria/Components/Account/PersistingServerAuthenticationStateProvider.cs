using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Sample.Pizzeria.Client;

namespace Sample.Pizzeria.Components.Account;

// This is a server-side AuthenticationStateProvider that uses PersistentComponentState to flow the
// authentication state to the client which is then fixed for the lifetime of the WebAssembly application.
internal sealed class PersistingServerAuthenticationStateProvider : ServerAuthenticationStateProvider, IDisposable
{
    private readonly PersistentComponentState state;
    private readonly IdentityOptions options;

    private readonly PersistingComponentStateSubscription subscription;

    private Task<AuthenticationState>? authenticationStateTask;

    public PersistingServerAuthenticationStateProvider(
        PersistentComponentState persistentComponentState,
        IOptions<IdentityOptions> optionsAccessor)
    {
        this.state = persistentComponentState;
        this.options = optionsAccessor.Value;

        this.AuthenticationStateChanged += this.OnAuthenticationStateChanged;
        this.subscription = this.state.RegisterOnPersisting(this.OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    public void Dispose()
    {
        this.subscription.Dispose();
        this.AuthenticationStateChanged -= this.OnAuthenticationStateChanged;
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        this.authenticationStateTask = task;
    }

    private async Task OnPersistingAsync()
    {
        if (this.authenticationStateTask is null)
        {
            throw new UnreachableException($"Authentication state not set in {nameof(this.OnPersistingAsync)}().");
        }

        var authenticationState = await this.authenticationStateTask;
        var principal = authenticationState.User;

        if (principal.Identity?.IsAuthenticated == true)
        {
            var userId = principal.FindFirst(this.options.ClaimsIdentity.UserIdClaimType)?.Value;
            var email = principal.FindFirst(this.options.ClaimsIdentity.EmailClaimType)?.Value;

            if (userId != null && email != null)
            {
                this.state.PersistAsJson(nameof(UserInfo), new UserInfo
                {
                    UserId = userId,
                    Email = email,
                });
            }
        }
    }
}
