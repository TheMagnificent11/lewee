using System.Diagnostics.CodeAnalysis;

namespace Pizzeria.Store.Api.Orders;

/// <summary>
/// Empty request for endpoints that don't require parameters.
/// </summary>
[SuppressMessage(
    "SonarAnalyzer.CSharp",
    "S2094:Classes should not be empty",
    Justification = "Required for FastEndpoints CommandEndpoint pattern")]
internal sealed record EmptyRequest;
