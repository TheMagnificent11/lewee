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
    // Resource log lines are flushed to Aspire's ResourceLoggerService asynchronously, so
    // WaitForNewCorrelationIdsAsync polls at this interval rather than reading a single snapshot.
    private const int PollingDelaySeconds = 2;

    // Total time to wait for correlation-scoped log lines to be flushed and captured.
    private const int PollingTimeoutSeconds = 30;

    // Both applications participate in the correlated request: the web client generates the
    // correlation ID and the API inherits it via the Correlate HTTP client propagation.
    private static readonly string[] ServicesToInspect =
    [
        ServiceNames.PizzaStoreWeb,
        ServiceNames.PizzaStoreApi,
    ];

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

        // Take a snapshot of the log lines written so far for every service involved in the request,
        // so we only inspect the ones written while starting the order (a new correlation ID is
        // generated on the web client for this action and flows through to the API).
        var logLinesBeforeOrderStarted = await this.GetLogLinesAsync(ServicesToInspect);

        // Act
        await playwrightPage.Page.ClickAsync(Home.Selectors.StartOrderButton);
        await this.WaitForDomainEventsToBeDispatchedAsync();

        await playwrightPage.Page.WaitForURLAsync(
            url => url.Contains("/orders/", StringComparison.OrdinalIgnoreCase),
            new PageWaitForURLOptions { Timeout = 30000 });

        // Log lines are flushed to the resource's console output asynchronously, so poll for them
        // rather than reading a single snapshot immediately after the request completes.
        var correlationIds = await this.WaitForNewCorrelationIdsAsync(logLinesBeforeOrderStarted);

        // Assert
        correlationIds.Should().NotBeEmpty(
            "the web client and API should write log messages scoped with the correlation ID generated when the order was started");

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

    private async Task<Dictionary<string, int>> GetLogLinesAsync(IEnumerable<string> serviceNames)
    {
        var logLineCounts = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var serviceName in serviceNames)
        {
            var logLines = await this.Factory.GetResourceLogLinesAsync(serviceName);
            logLineCounts[serviceName] = logLines.Count;
        }

        return logLineCounts;
    }

    private async Task<List<Guid>> WaitForNewCorrelationIdsAsync(Dictionary<string, int> logLineCountsBeforeOrderStarted)
    {
        var timeout = TimeSpan.FromSeconds(PollingTimeoutSeconds);
        var delay = TimeSpan.FromSeconds(PollingDelaySeconds);
        var startTime = DateTime.UtcNow;

        while (DateTime.UtcNow - startTime < timeout)
        {
            var correlationIds = await this.GetNewCorrelationIdsAsync(logLineCountsBeforeOrderStarted);

            if (correlationIds.Count > 0)
            {
                return correlationIds;
            }

            await Task.Delay(delay);
        }

        // Return whatever was found on the final attempt, so the caller gets a meaningful
        // (possibly empty) result rather than a generic timeout exception.
        return await this.GetNewCorrelationIdsAsync(logLineCountsBeforeOrderStarted);
    }

    private async Task<List<Guid>> GetNewCorrelationIdsAsync(Dictionary<string, int> logLineCountsBeforeOrderStarted)
    {
        var newLogLines = new List<string>();

        foreach (var serviceName in ServicesToInspect)
        {
            var currentLogLines = await this.Factory.GetResourceLogLinesAsync(serviceName);

            // Falls back to 0 if a service wasn't present in the earlier snapshot (e.g. it wasn't
            // included in the ServicesToInspect list at the time GetLogLinesAsync was called).
            var previousCount = logLineCountsBeforeOrderStarted.GetValueOrDefault(serviceName);

            newLogLines.AddRange(currentLogLines.Skip(previousCount));
        }

        return ExtractCorrelationIds(newLogLines);
    }
}
