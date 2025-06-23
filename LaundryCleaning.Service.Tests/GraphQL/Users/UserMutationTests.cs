using FluentAssertions;
using HotChocolate.Subscriptions;
using LaundryCleaning.Models.Subscriptions;
using LaundryCleaning.Service.Data;
using LaundryCleaning.Service.GraphQL.Users.Inputs;
using LaundryCleaning.Service.GraphQL.Users.Services.Implementations;
using LaundryCleaning.Service.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace LaundryCleaning.Service.Tests.GraphQL.Users
{
    public class UserMutationTests
    {
        [Fact]
        public async Task Mutation_CreateUser()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext)null);

            var dbContext = new ApplicationDbContext(options, httpContextAccessor.Object);

            var passwordService = new Mock<IPasswordService>();
            passwordService.Setup(p => p.GeneratePassword(It.IsAny<string>()))
                   .Returns("hashed-password");

            var topicSender = new Mock<ITopicEventSender>();
            var logger = new Mock<ILogger<UserService>>();
            var publisher = new Mock<IPublisherService>();

            var userService = new UserService(
                dbContext,
                passwordService.Object,
                topicSender.Object,
                logger.Object,
                publisher.Object
            );

            var input = new CreateUserInput
            {
                Email = "test@example.com",
                Password = "123456",
                Username = "testuser",
                FirstName = "Iyon",
                LastName = "Adryan"
            };

            // Act
            var result = await userService.CreateUser(input, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Email.Should().Be("test@example.com");

            var userInDb = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == input.Email);
            userInDb.Should().NotBeNull();
            userInDb.Password.Should().Be("hashed-password");

            topicSender.Verify(t =>
                t.SendAsync<UserCreated>(
                    "OnUserCreated",
                    It.IsAny<UserCreated>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
