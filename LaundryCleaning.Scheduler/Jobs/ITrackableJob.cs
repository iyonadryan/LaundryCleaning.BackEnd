namespace LaundryCleaning.Scheduler.Jobs
{
    public interface ITrackableJob
    {
        string JobName { get; } // for matching argument
        Task RunAsync(CancellationToken cancellationToken);
    }
}
