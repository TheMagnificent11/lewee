using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Lewee.Auth.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Pizzeria.Ordering.StateManagement;

namespace Pizzeria.Ordering.Web.Infrastructure;

[SuppressMessage(
    "Design",
    "CA1515:Consider making public types internal",
    Justification = "Blazor components must be public to be rendered")]
[SuppressMessage(
    "Usage",
    "VSTHRD003:Avoid awaiting foreign Tasks",
    Justification = "Authentication state tasks are supplied by Blazor's cascading authentication state.")]
public sealed class AuthenticatedUserInitializer : ComponentBase
{
    private string? attemptedUserId;

    [CascadingParameter]
    private Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

    [Inject]
    private BrowserAuthenticatedUserService AuthenticatedUserService { get; set; } = null!;

    [Inject]
    private ClientLifetimeCancellationTokenProvider CancellationTokenProvider { get; set; } = null!;

    [Inject]
    private IBffApiClient BffApiClient { get; set; } = null!;

    [Inject]
    private ILogger<AuthenticatedUserInitializer> Logger { get; set; } = null!;

    protected override async Task OnParametersSetAsync()
    {
        var authenticationState = await this.AuthenticationStateTask;
        var user = authenticationState.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        this.AuthenticatedUserService.SetUserId(userId);
        this.CancellationTokenProvider.UpdateAuthenticationState(user.Identity?.IsAuthenticated == true);

        if (string.IsNullOrEmpty(userId))
        {
            this.attemptedUserId = null;
            return;
        }

        if (this.attemptedUserId == userId)
        {
            return;
        }

        this.attemptedUserId = userId;

        try
        {
            await this.BffApiClient.CreateUserAsync(
                new CreateUserRequest { ExternalUserId = userId },
                this.CancellationTokenProvider.CancellationToken);
        }
        catch (Exception exception)
        {
            this.attemptedUserId = null;
            this.Logger.LogError(
                exception,
                "Failed to create user during first login for external user {ExternalUserId}",
                userId);
        }
    }
}
