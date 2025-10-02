namespace Lewee.Blazor.Tests.Integration;

public sealed class TestHttpClient
{
    private readonly HttpClient httpClient;

    public TestHttpClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<bool> GetHealthAsync()
    {
        using var response = await this.httpClient.GetAsync("/health");

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CreatePizzaOrderAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/orders");
        using var response = await this.httpClient.SendAsync(request);

        return response.IsSuccessStatusCode;
    }
}
