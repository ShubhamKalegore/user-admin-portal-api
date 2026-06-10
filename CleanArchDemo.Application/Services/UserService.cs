using CleanArchDemo.Application.DTOs;
using CleanArchDemo.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanArchDemo.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<List<UserResponseDto>> GetAllUsersAsync()
    {
        _logger.LogInformation("Fetching all users.");

        var users = await _userRepository.GetAllAsync();

        return users.Select(user => new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role
        }).ToList();
    }
}
