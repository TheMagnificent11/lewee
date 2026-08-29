using Microsoft.Extensions.Logging;

namespace Lewee.Auth.Application;

internal static partial class CreateUserCommandLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "User {UserId} created with external ID {ExternalId}")]
    public static partial void LogUserCreated(this ILogger logger, Guid userId, string externalId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "User {UserId} already exists with external ID {ExternalId}")]
    public static partial void LogUserAlreadyExists(this ILogger logger, Guid userId, string externalId);
}
