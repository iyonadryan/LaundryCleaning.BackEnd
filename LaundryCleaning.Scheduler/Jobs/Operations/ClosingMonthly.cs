namespace LaundryCleaning.Scheduler.Jobs.Operations
{
    public class ClosingMonthly : ITrackableJob
    {
        private readonly ILogger<ClosingMonthly> _logger;

        public ClosingMonthly(ILogger<ClosingMonthly> logger)
        {
            _logger = logger;
        }

        public string JobName => "closingmonthly";

        public Task RunAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[{DateTime.Now}] Running ClosingMonthly Job...");
            return Task.CompletedTask;
        }
    }
}
