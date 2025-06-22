using LaundryCleaning.Service.Common.Domain;

namespace LaundryCleaning.Service.Common.Models.Entities
{
    public class SchedulerExecutionLog : EntityBase
    {
        public string JobName { get; set; } = default!;
        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
        public bool IsSuccess { get; set; } = false;
        public string? ErrorMessage { get; set; }
    }
}
