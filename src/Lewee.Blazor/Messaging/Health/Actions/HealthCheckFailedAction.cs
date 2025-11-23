using System.Diagnostics.CodeAnalysis;

namespace Lewee.Blazor.Messaging.Health.Actions;

[SuppressMessage(
    "Minor Code Smell",
    "S2094:Classes should not be empty",
    Justification = "Needed for Redux pattern")]
internal record HealthCheckFailedAction();
