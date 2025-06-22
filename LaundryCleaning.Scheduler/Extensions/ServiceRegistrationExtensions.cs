using LaundryCleaning.Scheduler.Jobs;
using LaundryCleaning.Scheduler.Jobs.Operations;
namespace LaundryCleaning.Scheduler.Extensions
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            #region Scheduler Operation
            services.AddSingleton<ITrackableJob, ClosingMonthly>();
            services.AddSingleton<ITrackableJob, ReportDaily>();
            #endregion

            return services;
        }
    }
}
