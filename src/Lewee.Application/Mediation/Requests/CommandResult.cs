using FluentValidation.Results;

namespace Lewee.Application.Mediation.Requests;

/// <summary>
/// Command Result
/// </summary>
public class CommandResult : Result
{
    private CommandResult(ResultStatus status, List<ValidationFailure>? errors)
        : base(status, errors)
    {
    }

    /// <summary>
    /// Successful <see cref="CommandResult"/>
    /// </summary>
    /// <returns>
    /// <see cref="CommandResult"/>
    /// </returns>
    public static CommandResult Success()
    {
        return new CommandResult(ResultStatus.Success, null);
    }

    /// <summary>
    /// Failure <see cref="CommandResult"/>
    /// </summary>
    /// <param name="status">
    /// Result status
    /// </param>
    /// <param name="errorMessage">
    /// Error message
    /// </param>
    /// <returns>
    /// <see cref="CommandResult"/>
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <see cref="ResultStatus"/> is <see cref="ResultStatus.Success"/> or <see cref="ResultStatus.NotApplicable"/>
    /// </exception>
    public static CommandResult Fail(ResultStatus status, string errorMessage)
    {
        CheckIfFailure(status);

        return new CommandResult(
            status,
            new List<ValidationFailure> { new(string.Empty, errorMessage) });
    }

    /// <summary>
    /// Failure <see cref="CommandResult"/>
    /// </summary>
    /// <param name="status">
    /// Result status
    /// </param>
    /// <param name="errors">
    /// Errors keyed by request property name
    /// </param>
    /// <returns>
    /// <see cref="CommandResult"/>
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <see cref="ResultStatus"/> is <see cref="ResultStatus.Success"/> or <see cref="ResultStatus.NotApplicable"/>
    /// </exception>
    public static CommandResult Fail(ResultStatus status, List<ValidationFailure> errors)
    {
        CheckIfFailure(status);

        return new CommandResult(status, errors);
    }
}
