using Serilog.Events;

namespace Lewee.Infrastructure.Logging;

internal class SeqSettings
{
    public bool IsEnabled { get; set; } = false;
    public string Uri { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public LogEventLevel MinimumLogEventLevel { get; set; } = LogEventLevel.Information;
}
