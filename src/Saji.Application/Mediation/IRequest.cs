namespace Saji.Application.Mediation;

/// <summary>
/// Request Interface
/// </summary>
public interface IRequest
{
    /// <summary>
    /// Gets the correlation
    /// </summary>
    Guid CorrelationId { get; }

    /// <summary>
    /// Gets the tenant ID
    /// </summary>
    Guid? TenantId { get; }
}
