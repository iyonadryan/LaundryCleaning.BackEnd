using LaundryCleaning.Scheduler.Config;
using LaundryCleaning.Scheduler.Jobs;
using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.Data;
using Serilog;

namespace LaundryCleaning.Scheduler
{
    public class Worker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<Worker> _logger;
        private readonly SchedulerConfig _config;
        private readonly IEnumerable<ITrackableJob> _jobs;

        public Worker(IServiceScopeFactory scopeFactory
            , ILogger<Worker> logger
            ,SchedulerConfig config ,
            IEnumerable<ITrackableJob> jobs)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _config = config;
            _jobs = jobs;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tasks = new List<Task>();

            foreach (var jobConfig in _config.CronJobs)
            {
                tasks.Add(RunCronJobAsync(jobConfig, stoppingToken));
            }

            Log.Information($"ReadCronJobAsync : {DateTime.Now}");
            await Task.WhenAll(tasks);
        }

        private async Task RunCronJobAsync(CronJobConfig config, CancellationToken cancellationToken)
        {
            var cron = Cronos.CronExpression.Parse(config.Schedule);
            var timeZone = TimeZoneInfo.Local;

            while (!cancellationToken.IsCancellationRequested)
            {
                var next = cron.GetNextOccurrence(DateTimeOffset.Now, timeZone);
                if (next.HasValue)
                {
                    var delay = next.Value - DateTimeOffset.Now;
                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay, cancellationToken);

                    foreach (var arg in config.Arguments)
                    {
                        var matchedJob = _jobs.FirstOrDefault(j =>
                            arg.TrimStart('-').Equals(j.JobName, StringComparison.OrdinalIgnoreCase));

                        if (matchedJob != null)
                        {
                            using var scope = _scopeFactory.CreateScope();
                            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                            var log = new SchedulerExecutionLog
                            {
                                JobName = matchedJob.JobName,
                                ExecutedAt = DateTime.UtcNow
                            };

                            try
                            {
                                _logger.LogInformation($"Executing job: {matchedJob.JobName}");

                                await matchedJob.RunAsync(cancellationToken);
                                log.IsSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Job {matchedJob.JobName} failed.");

                                log.IsSuccess = false;
                                log.ErrorMessage = ex.Message;
                            }

                            dbContext._schedulerLog.Add(log);
                            await dbContext.SaveChangesAsync(cancellationToken);
                        }
                        else
                        {
                            _logger.LogWarning($"No job found for argument: {arg}");
                        }
                    }
                }
            }
        }
    }
}
