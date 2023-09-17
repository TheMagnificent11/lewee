using Serilog;

namespace Lewee.Blazor.Messaging.Health;

internal class HealthCheckService
{
    private readonly HttpClient httpClient;
    private readonly ILogger logger;

    public HealthCheckService(HttpClient httpClient, ILogger logger)
    {
        this.httpClient = httpClient;
        this.logger = logger.ForContext<HealthCheckService>();
    }

    public async Task<bool> IsServerHealthy(CancellationToken cancellationToken = default)
    {
        this.logger.Debug("Checking server health {ServerBaseAddress}", this.httpClient.BaseAddress);

        try
        {
            var response = await this.httpClient.GetAsync("/health", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            this.logger.Error(ex, "Failed health check");
            return false;
        }
    }
}
