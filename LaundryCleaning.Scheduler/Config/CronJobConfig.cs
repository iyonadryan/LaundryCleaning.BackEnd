namespace LaundryCleaning.Scheduler.Config
{
    public class CronJobConfig
    {
        public List<string> Arguments { get; set; } = new();
        public string Schedule { get; set; } = "";
    }

    public class SchedulerConfig
    {
        public List<CronJobConfig> CronJobs { get; set; } = new();
    }
}
