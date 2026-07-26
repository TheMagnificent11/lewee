using System.Text.Json;
using FluentAssertions;
using Lewee.Common;
using Lewee.Playwright;
using Microsoft.Playwright;
using Pizzeria.Common;
using Pizzeria.Store.Components;
using Pizzeria.Tests.Integration.Infrastructure;
using Xunit;

namespace Pizzeria.Tests.Integration;

[Collection(PizzeriaApplicationFactory.CollectionName)]
public sealed class CorrelationIdLoggingTests : PizzeriaTests
{
    public CorrelationIdLoggingTests(PizzeriaApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_UseConsistentCorrelationId_AcrossRequestLifecycle_When_CorrelationIdGeneratedOnWebClient()
    {
        // Arrange
        var webClientUrl = await this.Factory.GetWebClientBaseUrlAsync();
        var (username, password, email) = UserHelper.GenerateTestUserCredentials();

        var playwright = await this.Factory.GetPlaywrightAsync();
        await using var playwrightPage = await playwright.CreatePlaywritePageAsync();

        await playwrightPage.Page.RegisterUserAsync(webClientUrl, username, password, email);
        playwrightPage.Page.ShouldHaveBannerHeading();

        await playwrightPage.Page.WaitForSelectorAsync(
            Home.Selectors.StartOrderButton,
            new PageWaitForSelectorOptions { Timeout = 30000 });

        // Take a snapshot of the log lines written so far, so we only inspect the ones written
        // while starting the order (a new correlation ID is generated on the web client for this action).
        var logLinesBeforeOrderStarted = await this.Factory.GetResourceLogLinesAsync(ServiceNames.PizzaStoreWeb);

        // Act
        await playwrightPage.Page.ClickAsync(Home.Selectors.StartOrderButton);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        await playwrightPage.Page.WaitForURLAsync(
            url => url.Contains("/orders/", StringComparison.OrdinalIgnoreCase),
            new PageWaitForURLOptions { Timeout = 30000 });

        var logLinesAfterOrderStarted = await this.Factory.GetResourceLogLinesAsync(ServiceNames.PizzaStoreWeb);
        var newLogLines = logLinesAfterOrderStarted.Skip(logLinesBeforeOrderStarted.Count);

        // Assert
        var correlationIds = ExtractCorrelationIds(newLogLines);

        correlationIds.Should().NotBeEmpty(
            "the web client should write log messages scoped with the correlation ID generated when the order was started");

        correlationIds.Distinct().Should().ContainSingle(
            "the correlation ID generated on the web client when starting the order should scope every subsequent log " +
            "message for that request, including the one raised when the server's completion event is received back");
    }

    private static List<Guid> ExtractCorrelationIds(IEnumerable<string> logLines)
    {
        var correlationIds = new List<Guid>();

        foreach (var logLine in logLines)
        {
            if (string.IsNullOrWhiteSpace(logLine))
            {
                continue;
            }

            if (!TryParseJson(logLine, out var document))
            {
                continue;
            }

            using (document)
            {
                if (!document.RootElement.TryGetProperty("Scopes", out var scopes)
                    || scopes.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (var scope in scopes.EnumerateArray())
                {
                    if (scope.TryGetProperty(LoggingConsts.CorrelationId, out var correlationIdElement)
                        && Guid.TryParse(correlationIdElement.GetString(), out var correlationId))
                    {
                        correlationIds.Add(correlationId);
                    }
                }
            }
        }

        return correlationIds;
    }

    private static bool TryParseJson(string logLine, out JsonDocument document)
    {
        try
        {
            document = JsonDocument.Parse(logLine);
            return true;
        }
        catch (JsonException)
        {
            document = default;
            return false;
        }
    }
}
