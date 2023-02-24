namespace Lewee.Shared;

/// <summary>
/// Result Status
/// </summary>
public enum ResultStatus
{
    /// <summary>
    /// Not Applicable/Not Completed
    /// </summary>
    NotApplicable = 0,

    /// <summary>
    /// Success
    /// </summary>
    Success = 1,

    /// <summary>
    /// Unauthenicated
    /// </summary>
    Unauthenticated = 2,

    /// <summary>
    /// Unauthorised
    /// </summary>
    Unauthorized = 3,

    /// <summary>
    /// Bad Request
    /// </summary>
    BadRequest = 4,

    /// <summary>
    /// Not Found
    /// </summary>
    NotFound = 5
}
