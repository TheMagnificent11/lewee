using System.Diagnostics.CodeAnalysis;
using Lewee.Blazor.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Blazor.Tests.Integration.Infrastructure;

internal static class TestingExtensions
{
    [SuppressMessage(
        "Reliability",
        "CA2000:Dispose objects before losing scope",
        Justification = "Only test code (this would normally handled by the DI container)")]
    public static IServiceCollection AddLeweeBlazorForTesting<TMapper>(
        this IServiceCollection services,
        HttpClient testHttpClient,
        bool useReduxDevTools = false)
        where TMapper : class, IMessageToActionMapper
    {
        var testHandler = new TestHttpMessageHandler(testHttpClient);
        return services.AddLeweeBlazor<TMapper>(
            testHttpClient.BaseAddress!,
            useReduxDevTools,
            testHandler);
    }
}
