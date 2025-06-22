using Serilog;

namespace LaundryCleaning.Scheduler.Jobs.Operations
{
    public class ClosingMonthly : ITrackableJob
    {
        public string JobName => "closingmonthly";

        public Task RunAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"[{DateTime.Now}] Running ClosingMonthly Job...");

            Log.Information($"[{DateTime.Now}] Running ClosingMonthly Job...");
            return Task.CompletedTask;
        }
    }
}
