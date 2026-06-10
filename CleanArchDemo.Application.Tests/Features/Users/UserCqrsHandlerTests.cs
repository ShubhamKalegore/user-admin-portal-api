using CleanArchDemo.Application.Features.Users.Commands;
using CleanArchDemo.Application.Features.Users.Queries;
using CleanArchDemo.Application.Interfaces;
using CleanArchDemo.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace CleanArchDemo.Application.Tests.Features.Users;

public class UserCqrsHandlerTests
{
    [Fact]
    public async Task GetUsersQueryHandler_ShouldReturnUsers()
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

        var handler = new GetUsersQueryHandler(
            repositoryMock.Object,
            Mock.Of<ILogger<GetUsersQueryHandler>>());

        var result = await handler.Handle(new GetUsersQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal("admin@test.com", result[0].Email);
        Assert.Equal("Admin", result[0].Role);
    }

    [Fact]
    public async Task CreateUserCommandHandler_ShouldHashPasswordAndAddUser()
    {
        User? addedUser = null;
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(repository => repository.ExistsAsync("new@test.com"))
            .ReturnsAsync(false);
        repositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<User>()))
            .Callback<User>(user => addedUser = user)
            .Returns(Task.CompletedTask);

        var handler = new CreateUserCommandHandler(
            repositoryMock.Object,
            Mock.Of<ILogger<CreateUserCommandHandler>>());

        var result = await handler.Handle(
            new CreateUserCommand("new@test.com", "Password123!", "Admin"),
            CancellationToken.None);

        Assert.Equal("new@test.com", result.Email);
        Assert.Equal("Admin", result.Role);
        Assert.NotNull(addedUser);
        Assert.NotEqual("Password123!", addedUser!.PasswordHash);
        Assert.False(string.IsNullOrWhiteSpace(addedUser.PasswordHash));
    }

    [Fact]
    public async Task CreateUserCommandHandler_WhenUserExists_ShouldThrow()
    {
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(repository => repository.ExistsAsync("existing@test.com"))
            .ReturnsAsync(true);

        var handler = new CreateUserCommandHandler(
            repositoryMock.Object,
            Mock.Of<ILogger<CreateUserCommandHandler>>());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(
                new CreateUserCommand("existing@test.com", "Password123!", "User"),
                CancellationToken.None));
    }
}
