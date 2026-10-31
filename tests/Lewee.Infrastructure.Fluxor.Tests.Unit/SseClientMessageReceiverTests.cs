using System.Net;
using FluentAssertions;
using Lewee.Infrastructure.Fluxor;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Lewee.Infrastructure.Fluxor.Tests.Unit;

public sealed class SseClientMessageReceiverTests
{
    [Fact]
    public async Task StartAsync_ShouldRequestEventsRelativeToConfiguredBaseAddress()
    {
        var requestUri = new TaskCompletionSource<Uri>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var handler = new BlockingHttpMessageHandler(requestUri);
        using var httpClient = new HttpClient(handler, disposeHandler: false)
        {
            BaseAddress = new Uri("https://gateway.example/pizza-store-web/_api/bff/"),
        };
        await using var receiver = new SseClientMessageReceiver(
            httpClient,
            NullLogger<SseClientMessageReceiver>.Instance);

        await receiver.StartAsync();
        var requestedUri = await requestUri.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await receiver.StopAsync();

        requestedUri.AbsolutePath.Should().Be("/pizza-store-web/_api/bff/events");
    }

    private sealed class BlockingHttpMessageHandler(TaskCompletionSource<Uri> requestUri) : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            requestUri.TrySetResult(request.RequestUri);
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
