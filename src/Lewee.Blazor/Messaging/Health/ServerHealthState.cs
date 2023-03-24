namespace Lewee.Blazor.Messaging.Health;

internal record ServerHealthState
{
    public const int MaxAttempts = 3;

    public bool IsHealthy { get; set; }
    public int Attempts { get; set; }
    public bool Failed => !this.IsHealthy && this.Attempts >= MaxAttempts;
}
