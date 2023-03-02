namespace Lewee.Contracts;

/// <summary>
/// Client Message Contract Interface
/// </summary>
public interface IClientMessageContract
{
    /// <summary>
    /// Gets or sets the correlation ID
    /// </summary>
    Guid CorrelationId { get; set; }
}
