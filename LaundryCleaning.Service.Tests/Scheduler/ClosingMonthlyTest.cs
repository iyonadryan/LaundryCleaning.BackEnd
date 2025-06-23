using LaundryCleaning.Scheduler.Jobs.Operations;
using Microsoft.Extensions.Logging;
using Moq;

namespace LaundryCleaning.Service.Tests.Scheduler
{
    public class ClosingMonthlyTest
    {
        [Fact]
        public async Task RunAsync_ClosingMonthly()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ClosingMonthly>>();
            var job = new ClosingMonthly(loggerMock.Object);

            // Act
            var result = job.RunAsync(CancellationToken.None);

            // Assert
            Assert.Same(Task.CompletedTask, result);

            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Running ClosingMonthly Job")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
