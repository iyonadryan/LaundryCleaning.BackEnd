namespace LaundryCleaning.Models.Messages
{
    public class UserNotification
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
