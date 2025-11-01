using MediatR;
using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.Data.Tests.Integration;

/// <summary>
/// Test handler for domain events to track if events are dispatched
/// </summary>
internal sealed class TestOrderSubmittedEventHandler : INotificationHandler<TestOrderSubmittedEvent>
{
    private static readonly List<TestOrderSubmittedEvent> ReceivedEventsList = new();

    private readonly ILogger<TestOrderSubmittedEventHandler> logger;

    public TestOrderSubmittedEventHandler(ILogger<TestOrderSubmittedEventHandler> logger)
    {
        this.logger = logger;
    }

    public static List<TestOrderSubmittedEvent> ReceivedEvents => ReceivedEventsList;

    public static void Reset()
    {
        ReceivedEventsList.Clear();
    }

    public Task Handle(TestOrderSubmittedEvent notification, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Received TestOrderSubmittedEvent for Order {OrderId}", notification.OrderId);
        ReceivedEventsList.Add(notification);
        return Task.CompletedTask;
    }
}
