using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.Data;

internal static partial class DomainEventDispatcherLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Could not deserialize DomainEventReference {Id}")]
    public static partial void LogCouldNotDeserializeDomainEventReference(this ILogger logger, Guid id);
}
