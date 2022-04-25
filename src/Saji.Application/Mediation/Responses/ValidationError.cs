namespace Saji.Application.Mediation.Responses;

/// <summary>
/// Validation Error
/// </summary>
public class ValidationError : BaseError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> class
    /// </summary>
    /// <param name="message">
    /// Error Message
    /// </param>
    /// <param name="errors">
    /// Validaiton errors ((key is the name of the property, value are the error messages)
    /// </param>
    public ValidationError(
        string message,
        Dictionary<string, string[]> errors)
        : base(message)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Gets the dictionary of errorr (key is the name of the property, value are the error messages)
    /// </summary>
    public Dictionary<string, string[]> Errors { get; }
}
