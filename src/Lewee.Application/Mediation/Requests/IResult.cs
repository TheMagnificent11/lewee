namespace Lewee.Application.Mediation.Requests;

/// <summary>
/// Result Interface
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets a value indicating whether request was successfully processed
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets the status for the result
    /// </summary>
    ResultStatus Status { get; }

    /// <summary>
    /// Gets a dictionary of error messages keyed by request property
    /// </summary>
    Dictionary<string, List<string>> Errors { get; }
}
