using CleanArchDemo.Application.DTOs;
using CleanArchDemo.Application.Interfaces;
using CleanArchDemo.Application.Services;
using CleanArchDemo.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace CleanArchDemo.Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _repositoryMock = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _service = new AuthService(
            _repositoryMock.Object,
            CreateConfiguration(),
            Mock.Of<ILogger<AuthService>>());
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnNull_WhenUserAlreadyExists()
    {
        var request = new UserDto
        {
            Email = "existing@test.com",
            Password = "Password@123",
            Role = "User"
        };

        _repositoryMock
            .Setup(repository => repository.ExistsAsync(request.Email))
            .ReturnsAsync(true);

        var result = await _service.RegisterAsync(request);

        Assert.Null(result);
        _repositoryMock.Verify(repository => repository.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ShouldHashPasswordAndAddUser_WhenUserDoesNotExist()
    {
        var request = new UserDto
        {
            Email = "new@test.com",
            Password = "Password@123",
            Role = "Admin"
        };

        _repositoryMock
            .Setup(repository => repository.ExistsAsync(request.Email))
            .ReturnsAsync(false);

        var result = await _service.RegisterAsync(request);

        Assert.NotNull(result);
        Assert.Equal("new@test.com", result.Email);
        Assert.Equal("Admin", result.Role);
        Assert.NotEqual(request.Password, result.PasswordHash);
        _repositoryMock.Verify(repository => repository.AddAsync(result), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        var request = new UserDto
        {
            Email = "missing@test.com",
            Password = "Password@123"
        };

        _repositoryMock
            .Setup(repository => repository.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        var result = await _service.LoginAsync(request);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsInvalid()
    {
        var user = CreateUser("user@test.com", "CorrectPassword@123");
        var request = new UserDto
        {
            Email = user.Email,
            Password = "WrongPassword@123"
        };

        _repositoryMock
            .Setup(repository => repository.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);

        var result = await _service.LoginAsync(request);

        Assert.Null(result);
        _repositoryMock.Verify(repository => repository.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnTokensAndSaveRefreshToken_WhenCredentialsAreValid()
    {
        var user = CreateUser("user@test.com", "Password@123");
        var request = new UserDto
        {
            Email = user.Email,
            Password = "Password@123"
        };

        _repositoryMock
            .Setup(repository => repository.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);

        var result = await _service.LoginAsync(request);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
        Assert.Equal(result.RefreshToken, user.RefreshToken);
        _repositoryMock.Verify(repository => repository.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task RefreshTokensAsync_ShouldReturnNull_WhenRefreshTokenIsExpired()
    {
        var user = CreateUser("user@test.com", "Password@123");
        user.RefreshToken = "old-refresh-token";
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(-1);

        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        var result = await _service.RefreshTokensAsync(new RefreshTokenRequestDto
        {
            UserId = user.Id,
            RefreshToken = user.RefreshToken
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task RefreshTokensAsync_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
    {
        var user = CreateUser("user@test.com", "Password@123");
        user.RefreshToken = "valid-refresh-token";
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(5);

        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        var result = await _service.RefreshTokensAsync(new RefreshTokenRequestDto
        {
            UserId = user.Id,
            RefreshToken = "valid-refresh-token"
        });

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
        Assert.NotEqual("valid-refresh-token", result.RefreshToken);
        _repositoryMock.Verify(repository => repository.UpdateAsync(user), Times.Once);
    }

    private static IConfiguration CreateConfiguration()
    {
        var configurationMock = new Mock<IConfiguration>();

        SetupConfigurationValue(
            configurationMock,
            "AppSettings:Token",
            "this-is-a-long-test-secret-key-for-hs512-token-generation-with-more-than-sixty-four-bytes");
        SetupConfigurationValue(configurationMock, "AppSettings:Issuer", "CleanArchDemo.Tests");
        SetupConfigurationValue(configurationMock, "AppSettings:Audience", "CleanArchDemo.Tests");

        return configurationMock.Object;
    }

    private static void SetupConfigurationValue(
        Mock<IConfiguration> configurationMock,
        string key,
        string value)
    {
        var sectionMock = new Mock<IConfigurationSection>();
        sectionMock.Setup(section => section.Value).Returns(value);
        configurationMock.Setup(configuration => configuration.GetSection(key)).Returns(sectionMock.Object);
    }

    private static User CreateUser(string email, string password)
    {
        var user = new User
        {
            Email = email,
            Role = "User"
        };

        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, password);
        return user;
    }
}
