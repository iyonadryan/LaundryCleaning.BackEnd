using GreenDonut;
using LaundryCleaning.Models.Messages;
using LaundryCleaning.Service.Common.Handlers;
using Microsoft.Extensions.Logging;
using Moq;

namespace LaundryCleaning.Service.Tests.Handlers
{
    public class UserNotificationHandlerTest
    {
        [Fact]
        public async Task HandleAsync_UserNotificationHandler()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<UserNotificationHandler>>();
            var handler = new UserNotificationHandler(mockLogger.Object);
            var message = new UserNotification { Message = "Hello there!" };

            // Act
            var result = handler.HandleAsync(message, CancellationToken.None);

            // Assert
            Assert.Same(Task.CompletedTask, result);

            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Notifikasi dari topik {nameof(UserNotification)}: {message.Message}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
