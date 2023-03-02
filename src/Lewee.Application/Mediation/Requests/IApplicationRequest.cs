namespace Lewee.Application.Mediation.Requests;

/// <summary>
/// Request Interface
/// </summary>
public interface IApplicationRequest
{
    /// <summary>
    /// Gets the correlation
    /// </summary>
    Guid CorrelationId { get; }
}
