using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Lewee.Application.Mediation.Behaviors;

[SuppressMessage(
    "Major Code Smell",
    "S3881:\"IDisposable\" should be implemented correctly",
    Justification = "Nothing to dispose, just requires `IDisposable` for `using` pattern")]
internal class TimedOperation : IDisposable
{
    public const string BeginningOperationTemplate = "Beginning operation {TimedOperation}";
    public const string CompletedOperationTemplate = "Completed operation {TimedOperation} in {TimedOperationElapsedMs} ms";

    private readonly Stopwatch stopwatch;

    private readonly ILogger logger;
    private readonly string timedOperationName;

    public TimedOperation(ILogger logger, string timedOperationName)
    {
        this.logger = logger;
        this.timedOperationName = timedOperationName;

        this.logger.LogBeginningOperation(this.timedOperationName);
        this.stopwatch = Stopwatch.StartNew();
    }

    public void Dispose()
    {
        this.stopwatch.Stop();
        this.logger.LogCompletedOperation(
            this.timedOperationName,
            this.stopwatch.ElapsedMilliseconds);
    }
}
