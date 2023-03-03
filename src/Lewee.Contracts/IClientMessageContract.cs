using System.Text.Json;

namespace Lewee.Contracts;

/// <summary>
/// Client Message Contract Interface
/// </summary>
public interface IClientMessageContract
{
}

/// <summary>
/// Client Message Contract Extension Methods
/// </summary>
public static class ClientMessageContractExtensions
{
    /// <summary>
    /// To client message
    /// </summary>
    /// <param name="message">Message to convert</param>
    /// <param name="correlationId">Correlation ID</param>
    /// <param name="clientId">Client ID</param>
    /// <returns>Client message</returns>
    public static ClientMessge ToClientMessage(
        this IClientMessageContract message,
        Guid correlationId,
        string clientId)
    {
        var messageType = message.GetType();

        return new ClientMessge
        {
            CorrelationId = correlationId,
            ClientId = clientId,
            ContractAssemblyName = messageType.Assembly.FullName ?? string.Empty,
            ContractFullClassName = messageType.FullName ?? string.Empty,
            MessageJson = JsonSerializer.Serialize(message, messageType)
        };
    }
}
