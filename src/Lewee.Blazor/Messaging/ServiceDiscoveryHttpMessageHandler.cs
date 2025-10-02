namespace Lewee.Blazor.Messaging;

/// <summary>
/// HTTP message handler that uses service discovery through a named HttpClient
/// </summary>
internal sealed class ServiceDiscoveryHttpMessageHandler : HttpMessageHandler
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly string httpClientName;

    public ServiceDiscoveryHttpMessageHandler(IHttpClientFactory httpClientFactory, string httpClientName)
    {
        this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        this.httpClientName = httpClientName ?? throw new ArgumentNullException(nameof(httpClientName));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Create a new HttpClient instance for each request to ensure proper service discovery
        using var httpClient = this.httpClientFactory.CreateClient(this.httpClientName);

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
            newRequest.Options.Set(new HttpRequestOptionsKey<object?>(property.Key), property.Value);
        }

        // Forward the request through the named HttpClient which uses service discovery
        return await httpClient.SendAsync(newRequest, cancellationToken);
    }
}
