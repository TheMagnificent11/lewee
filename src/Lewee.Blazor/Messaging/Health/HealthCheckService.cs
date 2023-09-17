using Microsoft.Extensions.Logging;

namespace Lewee.Blazor.Messaging.Health;

internal class HealthCheckService
{
    private readonly HttpClient httpClient;
    private readonly ILogger<HealthCheckService> logger;

    public HealthCheckService(HttpClient httpClient, ILogger<HealthCheckService> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<bool> IsServerHealthy(CancellationToken cancellationToken = default)
    {
        this.logger.LogDebug("Checking server health {ServerBaseAddress}", this.httpClient.BaseAddress);

        try
        {
            var response = await this.httpClient.GetAsync("/health", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed health check");
            return false;
        }
    }
}
