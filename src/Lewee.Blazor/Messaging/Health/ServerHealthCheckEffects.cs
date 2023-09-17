using Fluxor;
using Lewee.Blazor.Messaging.Health.Actions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Lewee.Blazor.Messaging.Health;

internal class ServerHealthCheckEffects
{
    private readonly IState<ServerHealthState> state;
    private readonly HealthCheckService healthCheckService;
    private readonly HubConnection hubConnection;
    private readonly ILogger<ServerHealthCheckEffects> logger;

    public ServerHealthCheckEffects(
        IState<ServerHealthState> state,
        HealthCheckService healthCheckService,
        HubConnection hubConnection,
        ILogger<ServerHealthCheckEffects> logger)
    {
        this.state = state;
        this.healthCheckService = healthCheckService;
        this.hubConnection = hubConnection;
        this.logger = logger;
    }

#pragma warning disable IDE0060 // Remove unused parameter
    [EffectMethod]
    public async Task CheckHealth(HealthCheckAction action, IDispatcher dispatcher)
    {
        this.logger.LogDebug("Checking server health...");

        try
        {
            var isHealthy = await this.healthCheckService.IsServerHealthy();

            if (isHealthy)
            {
                dispatcher.Dispatch(new HealthCheckSuccessAction());
                return;
            }

            dispatcher.Dispatch(new HealthCheckFailedAction());
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed health check");
            dispatcher.Dispatch(new HealthCheckFailedAction());
        }
    }

    [EffectMethod]
    public async Task HealthSuccess(HealthCheckAction action, IDispatcher dispatcher)
    {
        this.logger.LogDebug("Checking server health...success");

        await this.hubConnection.StartAsync();
    }

    [EffectMethod]
    public async Task HealthFailed(HealthCheckFailedAction action, IDispatcher dispatcher)
    {
        this.logger.LogDebug("Checking server health...failed");

        if (this.state.Value.Failed)
        {
            return;
        }

        await Task.Delay(TimeSpan.FromSeconds(3));

        dispatcher.Dispatch(new HealthCheckAction());
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
