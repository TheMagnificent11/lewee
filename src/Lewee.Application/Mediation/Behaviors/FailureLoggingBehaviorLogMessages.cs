using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace Lewee.Application.Mediation.Behaviors;

internal static partial class FailureLoggingBehaviorLogMessages
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Unexpected error occurred {@Errors}")]
    public static partial void LogUnexpectedError(this ILogger logger, IList<ValidationFailure> errors);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Bad request {@Errors}")]
    public static partial void LogBadRequest(this ILogger logger, IList<ValidationFailure> errors);
}
