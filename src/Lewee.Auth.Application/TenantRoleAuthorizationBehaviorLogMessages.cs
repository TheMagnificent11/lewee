using Microsoft.Extensions.Logging;

namespace Lewee.Auth.Application;

internal static partial class TenantRoleAuthorizationBehaviorLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Tenant role authorization failed - no authenticated caller for {RequestType}")]
    public static partial void LogTenantRoleUnauthenticated(this ILogger logger, string requestType);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Tenant role authorization bypassed - caller {ExternalId} is a site administrator for tenant {TenantId} for {RequestType}")]
    public static partial void LogTenantRoleSiteAdministratorOverride(this ILogger logger, string externalId, Guid tenantId, string requestType);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Tenant role authorization failed - caller {ExternalId} does not satisfy required roles for tenant {TenantId} for {RequestType}")]
    public static partial void LogTenantRoleUnauthorized(this ILogger logger, string externalId, Guid tenantId, string requestType);
}
