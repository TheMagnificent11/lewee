namespace Saji.Application.Mediation.Responses;

/// <summary>
/// Command Result
/// </summary>
public class CommandResult : BaseResult
{
    private CommandResult(
        ResultStatus status,
        Dictionary<string, string[]>? errors)
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

        var errors = new Dictionary<string, string[]>()
        {
            { string.Empty, new string[] { errorMessage } }
        };

        return new CommandResult(status, errors);
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
    public static CommandResult Fail(ResultStatus status, Dictionary<string, string[]> errors)
    {
        CheckIfFailure(status);

        return new CommandResult(status, errors);
    }
}
