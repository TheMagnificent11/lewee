using Serilog.Events;

namespace Lewee.Infrastructure.Logging;

/// <summary>
/// Seq Settings
/// </summary>
public class SeqSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether Seq is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// Gets or sets the Seq URI
    /// </summary>
    public string Uri { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Seq API key
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the minimum log event level
    /// </summary>
    /// <remarks>
    /// Serilog minimum log event level will be Debug and this setting is for Seq
    /// </remarks>
    public LogEventLevel MinimumLogEventLevel { get; set; } = LogEventLevel.Information;
}
