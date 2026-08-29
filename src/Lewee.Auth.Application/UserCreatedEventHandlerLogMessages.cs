using Microsoft.Extensions.Logging;

namespace Lewee.Auth.Application;

internal static partial class UserCreatedEventHandlerLogMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Handling UserCreatedEvent for user {UserId}")]
    public static partial void LogHandlingUserCreatedEvent(this ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Published UserDto for user {UserId}")]
    public static partial void LogPublishedUserDto(this ILogger logger, Guid userId);
}
