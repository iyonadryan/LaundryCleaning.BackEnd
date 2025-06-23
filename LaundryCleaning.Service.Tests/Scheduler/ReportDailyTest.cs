using LaundryCleaning.Scheduler.Jobs.Operations;
using Microsoft.Extensions.Logging;
using Moq;

namespace LaundryCleaning.Service.Tests.Scheduler
{
    public class ReportDailyTest
    {
        [Fact]
        public async Task RunAsync_ReportDaily()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ReportDaily>>();
            var job = new ReportDaily(loggerMock.Object);

            // Act
            var result = job.RunAsync(CancellationToken.None);

            // Assert
            Assert.Same(Task.CompletedTask, result);

            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Running ReportDaily Job")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
