using Bunit;
using Correlate;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MudBlazor.Services;
using Pizzeria.Store.Web.Services;
using Pizzeria.Store.Web.States.Orders;

namespace Pizzeria.Store.Web.Tests.Unit;

public static class TestContextExtensions
{
    public static void Setup(this TestContext testContext)
    {
        var mockApiClient = new Mock<IPizzeriaApiClient>();
        var mockCorrelationContextAccessor = new Mock<ICorrelationContextAccessor>();
        var mockLogger = new Mock<ILogger<OrdersEffects>>();

        testContext.Services.AddSingleton(mockApiClient.Object);
        testContext.Services.AddSingleton(mockCorrelationContextAccessor.Object);
        testContext.Services.AddSingleton(mockLogger.Object);
        testContext.Services.AddMudServices();
        testContext.Services.AddFluxor(o => o.ScanAssemblies(typeof(OrdersState).Assembly));
    }
}
