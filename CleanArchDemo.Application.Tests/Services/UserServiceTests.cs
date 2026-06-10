using CleanArchDemo.Application.Interfaces;
using CleanArchDemo.Application.Services;
using CleanArchDemo.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace CleanArchDemo.Application.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnUserResponseDtos()
    {
        var users = new List<User>
        {
            new() { Email = "admin@test.com", Role = "Admin" },
            new() { Email = "user@test.com", Role = "User" }
        };

        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(users);

        var service = new UserService(
            repositoryMock.Object,
            Mock.Of<ILogger<UserService>>());

        var result = await service.GetAllUsersAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("admin@test.com", result[0].Email);
        Assert.Equal("Admin", result[0].Role);
        Assert.DoesNotContain(result, user => string.IsNullOrWhiteSpace(user.Email));
    }
}
