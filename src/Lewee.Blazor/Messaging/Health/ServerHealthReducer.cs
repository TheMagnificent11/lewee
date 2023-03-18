using Fluxor;
using Lewee.Blazor.Messaging.Health.Actions;

namespace Lewee.Blazor.Messaging.Health;

internal static class ServerHealthReducer
{
#pragma warning disable IDE0060 // Remove unused parameter
    [ReducerMethod]
    public static ServerHealthState OnHealthCheckSuccess(ServerHealthState state, HealthCheckSuccessAction action) =>
        state with { Attempts = state.Attempts + 1, IsHealthy = true };

    [ReducerMethod]
    public static ServerHealthState OnHealthCheckFailed(ServerHealthState state, HealthCheckFailedAction action) =>
        state with { Attempts = state.Attempts + 1, IsHealthy = false };
#pragma warning restore IDE0060 // Remove unused parameter
}
