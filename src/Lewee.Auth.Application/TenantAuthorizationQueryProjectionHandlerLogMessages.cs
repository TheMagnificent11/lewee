using Microsoft.Extensions.Logging;

namespace Lewee.Auth.Application;

internal static partial class TenantAuthorizationQueryProjectionHandlerLogMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Handling {DomainEventName} for user {UserId} and tenant {TenantId}")]
    public static partial void LogHandlingDomainEvent(this ILogger logger, string domainEventName, Guid userId, Guid tenantId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "User {UserId} not found while updating tenant authorization lookup")]
    public static partial void LogUserNotFound(this ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Updated tenant authorization lookup for user {UserId} and tenant {TenantId}")]
    public static partial void LogUpdatedProjection(this ILogger logger, Guid userId, Guid tenantId);
}
