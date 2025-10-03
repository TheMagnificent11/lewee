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
    public async Task CheckHealthAsync(HealthCheckAction action, IDispatcher dispatcher)
    {
        this.logger.LogDebug("Checking server health...");

        try
        {
            var isHealthy = await this.healthCheckService.IsServerHealthyAsync();

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
    public async Task HealthSuccessAsync(HealthCheckSuccessAction action, IDispatcher dispatcher)
    {
        this.logger.LogInformation("Server health check succeeded. Starting SignalR connection...");

        try
        {
            if (this.hubConnection.State == HubConnectionState.Disconnected)
            {
                this.logger.LogInformation("SignalR connection state is Disconnected. Starting connection...");
                await this.hubConnection.StartAsync();
                this.logger.LogInformation("SignalR hub connection started successfully. State: {State}", this.hubConnection.State);
            }
            else
            {
                this.logger.LogInformation("SignalR hub connection already in state: {State}", this.hubConnection.State);
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to start SignalR hub connection. Connection state: {State}", this.hubConnection.State);
            // Retry health check if connection fails
            dispatcher.Dispatch(new HealthCheckFailedAction());
        }
    }

    [EffectMethod]
    public async Task HealthFailedAsync(HealthCheckFailedAction action, IDispatcher dispatcher)
    {
        this.logger.LogWarning("Server health check failed. Attempts: {Attempts}/{MaxAttempts}", this.state.Value.Attempts, ServerHealthState.MaxAttempts);

        if (this.state.Value.Failed)
        {
            this.logger.LogError("Max health check attempts reached. Giving up.");
            return;
        }

        await Task.Delay(TimeSpan.FromSeconds(3));

        dispatcher.Dispatch(new HealthCheckAction());
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
