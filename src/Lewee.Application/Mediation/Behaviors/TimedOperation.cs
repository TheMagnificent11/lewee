using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Lewee.Application.Mediation.Behaviors;

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
