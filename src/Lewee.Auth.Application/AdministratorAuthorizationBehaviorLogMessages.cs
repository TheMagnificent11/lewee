using Microsoft.Extensions.Logging;

namespace Lewee.Auth.Application;

internal static partial class AdministratorAuthorizationBehaviorLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Administrator authorization failed - no authenticated caller for {RequestType}")]
    public static partial void LogAdministratorUnauthenticated(this ILogger logger, string requestType);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Administrator authorization failed - caller {ExternalId} is not a site administrator for {RequestType}")]
    public static partial void LogAdministratorUnauthorized(this ILogger logger, string externalId, string requestType);
}
