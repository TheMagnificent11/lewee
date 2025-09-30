using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Blazor.Tests.Integration;

public sealed class TestServerFixture : IDisposable
{
    public const string CollectionName = "TestServerCollection";

    private readonly TestServer server;

    private bool disposedValue;

    public TestServerFixture()
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services
                    .AddFakeLogging()
                    .AddRouting()
                    .AddLeweeBlazor<MessageToActionMapper>(new Uri("http://localhost"), useReduxDevTools: false)
                    .AddHealthChecks();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapHealthChecks("/health");
                });
            });

        this.server = new TestServer(builder);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public HttpClient CreateClient() => this.server.CreateClient();

    private void Dispose(bool disposing)
    {
        if (this.disposedValue)
        {
            return;
        }

        if (disposing)
        {
            this.server.Dispose();
        }

        this.disposedValue = true;
    }
}
