using System.Diagnostics.CodeAnalysis;
using Bunit;
using Correlate;
using FluentAssertions;
using Fluxor;
using Lewee.Common;
using Lewee.Infrastructure.Fluxor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MudBlazor.Services;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.StateManagement;
using Pizzeria.Store.Web;
using Xunit;

namespace Pizzeria.Store.Web.Tests.Unit;

[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test context handles disposal")]
[SuppressMessage("Reliability", "CA2213:Disposable fields should be disposed", Justification = "Test context handles disposal")]
public class MainLayoutTests : TestContext
{
    private readonly TestSseClientMessageReceiver testMessageReceiver;

    public MainLayoutTests()
    {
        var httpClient = new HttpClient();
        var logger = Mock.Of<ILogger<SseClientMessageReceiver>>();
        this.testMessageReceiver = new TestSseClientMessageReceiver(httpClient, logger);

        this.Services.AddSingleton(Mock.Of<IBffApiClient>());
        this.Services.AddSingleton(Mock.Of<ICorrelationContextAccessor>());
        this.Services.AddSingleton<SseClientMessageReceiver>(this.testMessageReceiver);
        this.Services.AddSingleton(Mock.Of<IMessageToActionMapper>());
        this.Services.AddSingleton(Mock.Of<IAuthenticatedUserService>());
        this.Services.AddLogging();
        this.Services.AddMudServices();
        this.Services.AddFluxor(o => o.ScanAssemblies(typeof(StoreStateManagementConfiguration).Assembly));

        this.JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void MainLayout_WhenRendered_ShowsAppBarWithTitle()
    {
        // Act
        var component = this.RenderComponent<MainLayout>();

        // Assert
        component.Markup.Should().Contain("Lewee Pizzeria");
    }

    [Fact]
    public void MainLayout_WhenRendered_ShowsSignOutButton()
    {
        // Act
        var component = this.RenderComponent<MainLayout>();

        // Assert
        component.Markup.Should().Contain("aria-label=\"sign-out\"");
    }

    [Fact]
    public void MainLayout_WhenRendered_ShowsSignOutFormWithCorrectAction()
    {
        // Act
        var component = this.RenderComponent<MainLayout>();

        // Assert
        component.Markup.Should().Contain($"action=\"{PageRoutes.SignOut}\"");
        component.Markup.Should().Contain("method=\"post\"");
    }

    /// <summary>
    /// Test implementation of SseClientMessageReceiver that does not connect
    /// </summary>
    [SuppressMessage("Reliability", "CA2215:Dispose methods should call base class dispose", Justification = "Test double - no resources to dispose")]
    private sealed class TestSseClientMessageReceiver : SseClientMessageReceiver
    {
        public TestSseClientMessageReceiver(HttpClient httpClient, ILogger<SseClientMessageReceiver> logger)
            : base(httpClient, logger)
        {
        }

        public override Task StartAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public override ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
