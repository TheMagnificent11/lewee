using System.Reflection;
using System.Text.Json;
using Lewee.Contracts;
using Microsoft.Extensions.Logging;

namespace Lewee.Blazor.Messaging;

internal class MessageDeserializer
{
    private const string CouldNotDeserializeError = "Could not deserialize message '{Message}'";
    private const string CouldNotFindContractError =
        "Could not find JSON contract type for message (Contract Assembly: {ContractAssembly}, Contract Class: {ContractClass})";

    private readonly ILogger<MessageDeserializer> logger;

    public MessageDeserializer(ILogger<MessageDeserializer> logger)
    {
        this.logger = logger;
    }

    public (object? messageBody, Guid? correlationId) Deserialize(ClientMessage clientMessage)
    {
        try
        {
            var assembly = Assembly.Load(clientMessage.ContractAssemblyName);
            var targetType = assembly.GetType(clientMessage.ContractFullClassName);
            if (targetType == null)
            {
                this.logger.LogError(
                    CouldNotFindContractError,
                    clientMessage.ContractAssemblyName,
                    clientMessage.ContractFullClassName);

                return (null, null);
            }

            var obj = JsonSerializer.Deserialize(clientMessage.MessageJson, targetType);
            if (obj == null)
            {
                this.logger.LogError(
                    CouldNotFindContractError,
                    clientMessage.ContractAssemblyName,
                    clientMessage.ContractFullClassName);
            }

            return (obj, clientMessage.CorrelationId);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, CouldNotDeserializeError, clientMessage);
            return (null, null);
        }
    }
}
