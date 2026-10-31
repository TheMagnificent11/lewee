using System.Security.Claims;
using Bunit;
using FluentAssertions;
using Lewee.Auth.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pizzeria.Ordering.StateManagement;
using Pizzeria.Ordering.Web.Infrastructure;
using Xunit;

namespace Pizzeria.Ordering.Web.Tests.Unit;

public sealed class AuthenticatedUserInitializerTests : TestContext
{
    [Fact]
    public void OnAuthenticationStateChange_WhenProvisioningInitiallyFails_RetriesProvisioning()
    {
        var user = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "external-user-id")], "test"));
        var authenticationStateProvider = new TestAuthenticationStateProvider(new AuthenticationState(user));
        var bffApiClient = new Mock<IBffApiClient>();
        var callCount = 0;
        bffApiClient
            .Setup(client => client.CreateUserAsync(It.IsAny<CreateUserRequest>(), It.IsAny<CancellationToken>()))
            .Returns(() => Interlocked.Increment(ref callCount) == 1
                ? Task.FromException(new HttpRequestException())
                : Task.CompletedTask);

        this.Services.AddSingleton<AuthenticationStateProvider>(authenticationStateProvider);
        this.Services.AddSingleton(bffApiClient.Object);
        this.Services.AddSingleton<BrowserAuthenticatedUserService>();
        this.Services.AddSingleton<ClientLifetimeCancellationTokenProvider>();
        this.Services.AddLogging();

        var component = this.RenderComponent<CascadingAuthenticationState>(parameters =>
            parameters.AddChildContent<AuthenticatedUserInitializer>());

        bffApiClient.Verify(
            client => client.CreateUserAsync(It.IsAny<CreateUserRequest>(), It.IsAny<CancellationToken>()),
            Times.Once);

        authenticationStateProvider.NotifyStateChanged();

        component.WaitForAssertion(() =>
            bffApiClient.Verify(
                client => client.CreateUserAsync(It.IsAny<CreateUserRequest>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2)));

        callCount.Should().Be(2);
    }

    private sealed class TestAuthenticationStateProvider(AuthenticationState authenticationState)
        : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(authenticationState);
        }

        public void NotifyStateChanged()
        {
            this.NotifyAuthenticationStateChanged(this.GetAuthenticationStateAsync());
        }
    }
}
