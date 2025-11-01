using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using FluentAssertions;
using Lewee.Domain;
using Lewee.Infrastructure.Data.Tests.App;
using Lewee.Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lewee.Infrastructure.Data.Tests.Integration;

/// <summary>
/// Integration tests for domain event dispatching
/// </summary>
public sealed class DomainEventDispatchingTests : IAsyncLifetime
{
    private IDistributedApplicationTestingBuilder builder;
    private DistributedApplication app;
    private string connectionString;
    private IServiceProvider serviceProvider;

    public async Task InitializeAsync()
    {
        // Create the test application
        this.builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Lewee_Infrastructure_Data_Tests_App>();

        this.app = await this.builder.BuildAsync();

        // Get resource notification service before starting
        var resourceNotificationService = this.app.Services.GetRequiredService<ResourceNotificationService>();

        await this.app.StartAsync();

        // Wait for PostgreSQL to be running
        await resourceNotificationService
            .WaitForResourceAsync(ServiceNames.DatabaseServer, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromMinutes(5));

        // Wait for database to be ready
        await resourceNotificationService
            .WaitForResourceAsync(ServiceNames.Database, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromMinutes(5));

        // Get connection string
        this.connectionString = await this.app.GetConnectionStringAsync(ServiceNames.Database);

        // Build service provider with necessary services
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DomainEventDispatchingTests).Assembly));
        services.AddSingleton<IAuthenticatedUserService>(new TestAuthenticatedUserService());

        services.AddLeweePostgreSQL<TestDbContext>(
            this.connectionString!,
            typeof(TestOrder).Assembly,
            "test");

        this.serviceProvider = services.BuildServiceProvider();

        // Retry database initialization with exponential backoff
        await this.WaitForDatabaseReadyAsync();
    }

    public async Task DisposeAsync()
    {
        if (this.serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        if (this.app != null)
        {
            await this.app.StopAsync();
            await this.app.DisposeAsync();
        }

        if (this.builder != null)
        {
            await this.builder.DisposeAsync();
        }
    }

    [Fact]
    public async Task DomainEvents_ShouldBeDispatchedAfterSaveChangesAsync()
    {
        // Arrange
        TestOrderSubmittedEventHandler.Reset();

        await using var scope = this.serviceProvider!.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();

        var orderId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var order = new TestOrder(orderId, "TEST-001");

        // Act
        order.Submit(correlationId);
        dbContext.Orders!.Add(order);
        await dbContext.SaveChangesAsync();

        // Assert - Event should be dispatched immediately after save
        TestOrderSubmittedEventHandler.ReceivedEvents.Should().ContainSingle();
        var receivedEvent = TestOrderSubmittedEventHandler.ReceivedEvents[0];
        receivedEvent.OrderId.Should().Be(orderId);
        receivedEvent.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public async Task DomainEvents_ShouldBeMarkedAsDispatchedAsync()
    {
        // Arrange
        TestOrderSubmittedEventHandler.Reset();

        await using var scope = this.serviceProvider!.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();

        var orderId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var order = new TestOrder(orderId, "TEST-002");

        // Act
        order.Submit(correlationId);
        dbContext.Orders!.Add(order);
        await dbContext.SaveChangesAsync();

        // Assert - Domain event reference should be marked as dispatched
        var eventReferences = await dbContext.DomainEventReferences!
            .Where(e => !e.Dispatched)
   .ToListAsync();

        eventReferences.Should().BeEmpty("all events should be dispatched");
    }

    [Fact]
    public async Task MultipleDomainEvents_ShouldAllBeDispatchedAsync()
    {
        // Arrange
        TestOrderSubmittedEventHandler.Reset();

        await using var scope = this.serviceProvider!.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();

        var order1 = new TestOrder(Guid.NewGuid(), "TEST-003");
        var order2 = new TestOrder(Guid.NewGuid(), "TEST-004");
        var order3 = new TestOrder(Guid.NewGuid(), "TEST-005");

        // Act
        order1.Submit(Guid.NewGuid());
        order2.Submit(Guid.NewGuid());
        order3.Submit(Guid.NewGuid());

        dbContext.Orders!.Add(order1);
        dbContext.Orders!.Add(order2);
        dbContext.Orders!.Add(order3);

        await dbContext.SaveChangesAsync();

        // Assert - All events should be dispatched
        TestOrderSubmittedEventHandler.ReceivedEvents.Should().HaveCount(3);
    }

    [Fact]
    public async Task DomainEvents_ShouldNotBeDispatchedIfSaveFailsAsync()
    {
        // Arrange
        TestOrderSubmittedEventHandler.Reset();

        // First, add an order to the database
        var orderId = Guid.NewGuid();
        await using (var scope = this.serviceProvider!.CreateAsyncScope())
        {
            var setupDbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            var existingOrder = new TestOrder(orderId, "TEST-006-EXISTING");
            setupDbContext.Orders!.Add(existingOrder);
            await setupDbContext.SaveChangesAsync();
        }

        // Now try to add another order with the same ID in a new context
        await using var testScope = this.serviceProvider!.CreateAsyncScope();
        var dbContext = testScope.ServiceProvider.GetRequiredService<TestDbContext>();

        var duplicateOrder = new TestOrder(orderId, "TEST-006-DUPLICATE");

        // Act
        duplicateOrder.Submit(Guid.NewGuid());
        dbContext.Orders!.Add(duplicateOrder);

        // Assert
        Func<Task> act = async () => await dbContext.SaveChangesAsync();

        await act.Should().ThrowAsync<DbUpdateException>();

        // Events should not be dispatched because save failed
        TestOrderSubmittedEventHandler.ReceivedEvents.Should().BeEmpty();
    }

    private async Task WaitForDatabaseReadyAsync()
    {
        var maxRetries = 10;
        var delayMs = 500;

        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                await using var scope = this.serviceProvider.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();

                await dbContext.Database.EnsureCreatedAsync();
                return; // Success
            }
            catch (Exception) when (i < maxRetries - 1)
            {
                // Wait with exponential backoff
                await Task.Delay(delayMs);
                delayMs *= 2; // Exponential backoff
            }
        }

        // One final attempt without catching exceptions
        await using var finalScope = this.serviceProvider.CreateAsyncScope();
        var finalDbContext = finalScope.ServiceProvider.GetRequiredService<TestDbContext>();
        await finalDbContext.Database.EnsureCreatedAsync();
    }
}
