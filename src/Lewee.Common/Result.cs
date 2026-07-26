using System.Diagnostics.CodeAnalysis;
using System.Text;
using FluentValidation.Results;

namespace Lewee.Common;

/// <summary>
/// Result
/// </summary>
public abstract class Result
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
    protected Result(ResultStatus status, IList<ValidationFailure>? errors)
    {
        this.Status = status;
        this.Errors = errors ?? [];
        this.IsSuccess = status == ResultStatus.Success;
    }

    /// <summary>
    /// Gets a value indicating whether request was successfully processed
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the status for the result
    /// </summary>
    public ResultStatus Status { get; }

    /// <summary>
    /// Gets a dictionary of error messages keyed by request property
    /// </summary>
    public IList<ValidationFailure> Errors { get; }

    /// <summary>
    /// Generates an error message from the <see cref="Errors"/> dictionary.
    /// </summary>
    /// <returns>Error message</returns>
    /// <remarks>Each string in the dictionary is separated by a new-line character</remarks>
    public string GenerateErrorMessage()
    {
        if (!this.Errors.Any())
        {
            return string.Empty;
        }

        var errorMessage = new StringBuilder();

        if (this.Errors.Count == 1)
        {
            return GenerateErrorMessage(this.Errors[0]);
        }

        this.Errors
            .ToList()
            .ForEach(x => errorMessage.AppendLine(GenerateErrorMessage(x)));

        return errorMessage.ToString();

        [SuppressMessage(
            "Minor Code Smell",
            "S3241:Methods should not return values that are never used",
            Justification = "False positive")]
        static string GenerateErrorMessage(ValidationFailure failure)
        {
            var message = string.IsNullOrWhiteSpace(failure.PropertyName)
                ? failure.ErrorMessage
                : $"{failure.PropertyName}: {failure.ErrorMessage}";

            return message;
        }
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
