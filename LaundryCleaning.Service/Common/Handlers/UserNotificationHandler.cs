using LaundryCleaning.Models.Messages;
using LaundryCleaning.Service.Services.Attributes;
using LaundryCleaning.Service.Services.Interfaces;

namespace LaundryCleaning.Service.Common.Handlers
{
    [SystemMessageHandlerFor(nameof(UserNotification))]
    public class UserNotificationHandler : ISystemMessageHandler<UserNotification>
    {
        private readonly ILogger<UserNotificationHandler> _logger;

        public UserNotificationHandler(ILogger<UserNotificationHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(UserNotification message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Notifikasi dari topik {nameof(UserNotification)}: {message.Message}");
            return Task.CompletedTask;
        }
    }
}
