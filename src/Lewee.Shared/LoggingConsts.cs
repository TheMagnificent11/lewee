namespace Lewee.Shared;

/// <summary>
/// Logging Constants
/// </summary>
public static class LoggingConsts
{
    /// <summary>
    /// Correlation ID
    /// </summary>
    public const string CorrelationId = "CorrelationId";

    /// <summary>
    /// Correlation ID Header
    /// </summary>
    public const string CorrelationIdHeaderKey = "correlationId";

    /// <summary>
    /// Tenant ID
    /// </summary>
    public const string TenantId = "TenantId";

    /// <summary>
    /// Request Type
    /// </summary>
    public const string RequestType = "RequestType";

    /// <summary>
    /// Test method to verify coverage threshold - this should not be covered by tests
    /// </summary>
    /// <param name="correlationId">Correlation ID to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    public static bool ValidateCorrelationId(string correlationId)
    {
        if (string.IsNullOrEmpty(correlationId))
        {
            return false;
        }

        // This logic should not be covered by tests
        return correlationId.Length > 5 && correlationId.Contains("-");
    }
}
