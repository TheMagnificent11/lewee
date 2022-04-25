namespace Saji.Application.Mediation.Responses;

/// <summary>
/// Not Found Error
/// </summary>
public class NotFoundError : BaseError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundError"/> class
    /// </summary>
    /// <param name="entityType">
    /// Entity type that was not found
    /// </param>
    /// <param name="entityId">
    /// ID of entity that was not found
    /// </param>
    public NotFoundError(string entityType, object entityId)
        : base($"Failed to retrieve {entityType} with ID '{entityId}'")
    {
    }
}
