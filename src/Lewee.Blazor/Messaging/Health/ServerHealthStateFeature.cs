using Fluxor;

namespace Lewee.Blazor.Messaging.Health;

internal class ServerHealthStateFeature : Feature<ServerHealthState>
{
    public override string GetName() => nameof(ServerHealthState);

    protected override ServerHealthState GetInitialState() => new();
}
