namespace Lewee.Infrastructure.Events;

/// <summary>
/// Service Bus Settings
/// </summary>
public class ServiceBusSettings
{
    /// <summary>
    /// Gets or sets the service bus type
    /// </summary>
    public ServiceBusType BusType { get; set; }

    /// <summary>
    /// Gets or sets the connection string or host
    /// </summary>
    public string ConnectionStringOrHost { get; set; } = string.Empty;
}
