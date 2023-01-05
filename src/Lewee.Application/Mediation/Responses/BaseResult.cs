namespace Lewee.Application.Mediation.Responses;

/// <summary>
/// Base Result
/// </summary>
public abstract class BaseResult : IResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseResult"/> class
    /// </summary>
    /// <param name="status">
    /// Result status
    /// </param>
    /// <param name="errors">
    /// Error messages (keyed by request property)
    /// </param>
    protected BaseResult(
        ResultStatus status,
        Dictionary<string, List<string>>? errors)
    {
        this.Status = status;
        this.Errors = errors ?? new Dictionary<string, List<string>>();
        this.IsSuccess = status == ResultStatus.Success;
    }

    /// <summary>
    /// Gets a value indicating whether request was successfully processed
    /// </summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// Gets the status for the result
    /// </summary>
    public ResultStatus Status { get; private set; }

    /// <summary>
    /// Gets a dictionary of error messages keyed by request property
    /// </summary>
    public Dictionary<string, List<string>> Errors { get; private set; }

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
