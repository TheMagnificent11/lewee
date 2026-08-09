#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using FluentAssertions;
using Xunit;
using static Lewee.Infrastructure.Fluxor.SseClientMessageReceiver;

namespace Lewee.Infrastructure.Fluxor.Tests.Unit;

[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test cleanup handles disposal")]
[SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "Test does not require disposal")]
public static class SseItemWrapperJsonDeserializationTests
{
    [Fact]
    [SuppressMessage("Performance", "CA1869:Cache and reuse 'JsonSerializerOptions' instances", Justification = "Test code")]
    public static void DirectDeserialization_Test_CamelCase_ClientMessage()
    {
        var json = """
            {
                "data":
                {
                    "correlationId":"a803a784-d4d6-4fee-870b-3f7a8a11f2ca",
                    "contractAssemblyName":"Pizzeria.Store.Contracts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                    "contractFullClassName":"Pizzeria.Store.Contracts.Orders.OrderDto",
                    "messageJson":"{\"Id\":\"ceffee2a-bee0-4e88-998c-92d300c3ae40\",\"UserId\":\"e6c6e90f-727f-4a9a-ad59-0113fc253b70\",\"StartedDateTime\":\"2026-01-31T22:00:32.453846Z\",\"SubmittedDateTime\":null,\"PreparedDateTime\":null,\"CompletedDateTime\":null,\"DeliveryAddress\":null,\"Pizzas\":[],\"TotalCost\":0}"
                    },
                "eventType":"Pizzeria.Store.Contracts.Orders.OrderDto",
                "eventId":null,
                "reconnectionInterval":null
            }
            """;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var wrapper = JsonSerializer.Deserialize<SseItemWrapper>(json, options);

        wrapper.Should().NotBeNull();
        wrapper.Data.Should().NotBeNull();
        wrapper.Data.CorrelationId.Should().NotBeEmpty();
        wrapper.Data.ContractAssemblyName.Should().NotBeNullOrWhiteSpace();
        wrapper.Data.ContractFullClassName.Should().NotBeNullOrWhiteSpace();
        wrapper.Data.MessageJson.Should().NotBeNullOrWhiteSpace();
    }
}
