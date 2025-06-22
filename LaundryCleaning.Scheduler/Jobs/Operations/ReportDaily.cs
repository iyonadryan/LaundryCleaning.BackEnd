using Serilog;

namespace LaundryCleaning.Scheduler.Jobs.Operations
{
    public class ReportDaily : ITrackableJob
    {
        public string JobName => "reportdaily";

        public Task RunAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"[{DateTime.Now}] Running ReportDaily Job...");

            Log.Information($"[{DateTime.Now}] Running ReportDaily Job...");
            return Task.CompletedTask;
        }
    }
}
