using LaundryCleaning.Service.Common.Domain;

namespace LaundryCleaning.Service.Common.Models.Entities
{
    public class SystemReceived : EntityBase
    {
        public string Topic { get; set; } = default!;
        public string Payload { get; set; } = default!;
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

        public bool IsProcessed { get; set; } = false;
        public DateTime? ProcessedAt { get; set; }
        public string? ErrorMessage { get; set; }
        public int RetryCount { get; set; } = 0;
    }
}
