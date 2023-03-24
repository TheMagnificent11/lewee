using System.Text;

namespace Lewee.Application.Mediation.Requests;

/// <summary>
/// Result
/// </summary>
public abstract class Result : IResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class
    /// </summary>
    /// <param name="status">
    /// Result status
    /// </param>
    /// <param name="errors">
    /// Error messages (keyed by request property)
    /// </param>
    protected Result(ResultStatus status, Dictionary<string, List<string>>? errors)
    {
        this.Status = status;
        this.Errors = errors ?? new Dictionary<string, List<string>>();
        this.IsSuccess = status == ResultStatus.Success;
    }

    /// <inheritdoc />
    public bool IsSuccess { get; private set; }

    /// <inheritdoc />
    public ResultStatus Status { get; private set; }

    /// <inheritdoc />
    public Dictionary<string, List<string>> Errors { get; private set; }

    /// <summary>
    /// Generates an error message from the <see cref="Errors"/> dictionary.
    /// </summary>
    /// <returns>Error message</returns>
    /// <remarks>Each string in the dictionay is separated by a new-line character</remarks>
    public string GenerateErrorMessage()
    {
        if (!this.Errors.Any())
        {
            return string.Empty;
        }

        var errorMessage = new StringBuilder();

        foreach (var error in this.Errors)
        {
            errorMessage.Append($"{error.Key}:");

            foreach (var subError in error.Value)
            {
                errorMessage.Append($" {subError},");
            }

            errorMessage.Remove(errorMessage.Length - 1, 1);
            errorMessage.AppendLine();
        }

        return errorMessage.ToString();
    }

    /// <summary>
    /// Checks if the status provided is a failure status
    /// </summary>
    /// <param name="status">
    /// Status to check
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <see cref="ResultStatus"/> is <see cref="ResultStatus.Success"/> or <see cref="ResultStatus.NotApplicable"/>
    /// </exception>
    protected static void CheckIfFailure(ResultStatus status)
    {
        if (status == ResultStatus.Success || status == ResultStatus.NotApplicable)
        {
            throw new InvalidOperationException("Status cannot be 'Success' or 'Not Applicable' for a failure result");
        }
    }
}
