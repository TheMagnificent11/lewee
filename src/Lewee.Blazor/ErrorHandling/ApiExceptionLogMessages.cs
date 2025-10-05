using Microsoft.Extensions.Logging;

namespace Lewee.Blazor.ErrorHandling;

internal static partial class ApiExceptionLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Unexpected response status (Status Code: {StatusCode})")]
    public static partial void LogUnexpectedResponseStatus(this ILogger logger, Exception exception, int statusCode);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Request failed (Status Code: {StatusCode}, Message: {BadRequestMessage}, Response Body: {BadRequestResponseBody})")]
    public static partial void LogBadRequestFailed(this ILogger logger, Exception exception, int statusCode, string badRequestMessage, string badRequestResponseBody);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Request failed (Status Code: {StatusCode}, Message: {ErrorMessage})")]
    public static partial void LogClientErrorFailed(this ILogger logger, Exception exception, int statusCode, string errorMessage);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Request failed due to server error (Status Code: {StatusCode}, Message: {Message})")]
    public static partial void LogServerErrorFailed(this ILogger logger, Exception exception, int statusCode, string message);
}
