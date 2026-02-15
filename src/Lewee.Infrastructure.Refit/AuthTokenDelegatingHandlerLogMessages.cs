using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.Refit;

internal static partial class AuthTokenDelegatingHandlerLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Adding Bearer token to request: {RequestUri}")]
    public static partial void LogAddingBearerToken(this ILogger logger, Uri requestUri);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Access token is null or empty for request: {RequestUri}")]
    public static partial void LogAccessTokenNullOrEmpty(this ILogger logger, Uri requestUri);
}
