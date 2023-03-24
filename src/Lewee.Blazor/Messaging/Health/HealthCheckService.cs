namespace Lewee.Blazor.Messaging.Health;
internal class HealthCheckService
{
    private readonly HttpClient httpClient;

    public HealthCheckService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<bool> IsServerHealthy(CancellationToken cancellationToken = default)
    {
        var response = await this.httpClient.GetAsync("/health", cancellationToken);

        return response.IsSuccessStatusCode;
    }
}
