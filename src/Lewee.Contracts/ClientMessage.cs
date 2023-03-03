namespace Lewee.Contracts;

/// <summary>
/// Client Message
/// </summary>
public class ClientMessage
{
    /// <summary>
    /// Gets or sets the correlation ID
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the assembly name of the JSON contract class
    /// </summary>
    public string ContractAssemblyName { get; set; }

    /// <summary>
    /// Gets or sets the full class name of the JSON contract class
    /// </summary>
    public string ContractFullClassName { get; set; }

    /// <summary>
    /// Gets or sets the message JSON
    /// </summary>
    public string MessageJson { get; set; }
}
