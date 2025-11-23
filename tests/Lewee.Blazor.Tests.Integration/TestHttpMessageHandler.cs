namespace Lewee.Blazor.Tests.Integration;

internal sealed class TestHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpClient httpClient;

    public TestHttpMessageHandler(HttpClient httpClient)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Create a new request message to avoid "request already sent" errors
        using var newRequest = new HttpRequestMessage(request.Method, request.RequestUri);

        // Copy headers
        foreach (var header in request.Headers)
        {
            newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        // Copy content if present
        if (request.Content != null)
        {
            var contentBytes = await request.Content.ReadAsByteArrayAsync(cancellationToken);
            newRequest.Content = new ByteArrayContent(contentBytes);

            // Copy content headers
            foreach (var header in request.Content.Headers)
            {
                newRequest.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        // Copy properties
        foreach (var property in request.Options)
        {
            newRequest.Options.Set(new HttpRequestOptionsKey<object>(property.Key), property.Value);
        }

        // Forward the new request through the HttpClient which uses the TestServer's message handler
        return await this.httpClient.SendAsync(newRequest, cancellationToken);
    }
}
