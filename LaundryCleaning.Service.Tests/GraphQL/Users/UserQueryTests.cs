using FluentAssertions;
using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.GraphQL.Users.Queries;
using LaundryCleaning.Service.GraphQL.Users.Services.Interfaces;
using Moq;

namespace LaundryCleaning.Service.Tests.GraphQL.Users
{
    public class UserQueryTests
    {
        [Fact]
        public async Task Query_GetUsers()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedUser = new List<User>() { new User { Id = userId, FirstName = "Iyon" } };

            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.GetUsers(CancellationToken.None))
                       .ReturnsAsync(expectedUser);

            var userQueries = new UserQueries();

            // Act
            var result = await userQueries.GetUsers(mockService.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].FirstName.Should().Be("Iyon");
        }
    }
}
