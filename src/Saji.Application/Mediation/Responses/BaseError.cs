namespace Saji.Application.Mediation.Responses;

/// <summary>
/// Base Error
/// </summary>
public abstract class BaseError : IResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseError"/> class
    /// </summary>
    /// <param name="message">
    /// Error message
    /// </param>
    protected BaseError(string message)
    {
        this.Message = message;
    }

    /// <summary>
    /// Gets or sets the error message
    /// </summary>
    public string Message { get; protected set; }
}
