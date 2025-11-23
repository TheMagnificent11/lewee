using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.Data.Tests.Integration;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Used via mediation")]
internal sealed class TestOrderSubmittedEventHandler : INotificationHandler<TestOrderSubmittedEvent>
{
    private readonly ILogger<TestOrderSubmittedEventHandler> logger;

    public TestOrderSubmittedEventHandler(ILogger<TestOrderSubmittedEventHandler> logger)
    {
        this.logger = logger;
    }

    public static List<TestOrderSubmittedEvent> ReceivedEvents { get; } = [];

    public static void Reset()
    {
        ReceivedEvents.Clear();
    }

    public Task Handle(TestOrderSubmittedEvent notification, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Received TestOrderSubmittedEvent for Order {OrderId}", notification.OrderId);
        ReceivedEvents.Add(notification);
        return Task.CompletedTask;
    }
}
