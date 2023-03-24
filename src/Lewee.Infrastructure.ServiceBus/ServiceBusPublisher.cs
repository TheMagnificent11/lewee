using Lewee.Domain;
using MassTransit;
using MediatR;
using Serilog;
using LoggingContext = Serilog.Context.LogContext;

namespace Lewee.Infrastructure.ServiceBus;

internal class ServiceBusPublisher<TBusEvent> : INotificationHandler<TBusEvent>
    where TBusEvent : class, INotification, IServiceBusEvent
{
    private readonly IBus serviceBus;
    private readonly ILogger logger;

    public ServiceBusPublisher(IBus serviceBus, ILogger logger)
    {
        this.serviceBus = serviceBus;
        this.logger = logger.ForContext<ServiceBusPublisher<TBusEvent>>();
    }

    public async Task Handle(TBusEvent notification, CancellationToken cancellationToken)
    {
        using (LoggingContext.PushProperty("CorrelationId", notification.CorrelationId))
        {
            var message = notification.ToMessage();

            await this.serviceBus.Publish(message, cancellationToken);

            this.logger.Information("Message published to service bus {@Message}", message);
        }
    }
}
