namespace LaundryCleaning.Scheduler.Jobs.Operations
{
    public class ReportDaily : ITrackableJob
    {
        private readonly ILogger<ReportDaily> _logger;

        public ReportDaily(ILogger<ReportDaily> logger)
        {
            _logger = logger;
        }

        public string JobName => "reportdaily";

        public Task RunAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[{DateTime.Now}] Running ReportDaily Job...");
            return Task.CompletedTask;
        }
    }
}
