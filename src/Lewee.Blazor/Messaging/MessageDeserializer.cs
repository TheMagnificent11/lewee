using System.Reflection;
using System.Text.Json;
using Lewee.Contracts;
using Serilog;

namespace Lewee.Blazor.Messaging;

internal class MessageDeserializer
{
    private const string CouldNotDeserializeError = "Could not deserialize message '{Message}'";
    private const string CouldNotFindContractError =
        "Could not find JSON contract type for message (Contract Assembly: {ContractAssembly}, Contract Class: {ContractClass})";

    private readonly ILogger logger;

    public MessageDeserializer(ILogger logger)
    {
        this.logger = logger.ForContext<MessageDeserializer>();
    }

    public (object? messageBody, Guid? correlationId) Deserialize(ClientMessage clientMessage)
    {
        try
        {
            var assembly = Assembly.Load(clientMessage.ContractAssemblyName);
            var targetType = assembly.GetType(clientMessage.ContractFullClassName);
            if (targetType == null)
            {
                this.logger.Error(
                    CouldNotFindContractError,
                    clientMessage.ContractAssemblyName,
                    clientMessage.ContractFullClassName);

                return (null, null);
            }

            var obj = JsonSerializer.Deserialize(clientMessage.MessageJson, targetType);
            if (obj == null)
            {
                this.logger.Error(
                    CouldNotFindContractError,
                    clientMessage.ContractAssemblyName,
                    clientMessage.ContractFullClassName);
            }

            return (obj, clientMessage.CorrelationId);
        }
        catch (Exception ex)
        {
            this.logger.Error(ex, CouldNotDeserializeError, clientMessage);
            return (null, null);
        }
    }
}
