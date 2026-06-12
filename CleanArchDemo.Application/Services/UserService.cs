using AutoMapper;
using CleanArchDemo.Application.DTOs;
using CleanArchDemo.Application.Interfaces;
using CleanArchDemo.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CleanArchDemo.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger, IMapper mapper)
    {
        _userRepository = userRepository;
        _logger = logger;
        _mapper = mapper;
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

    public async Task<bool> DeleteUser(UserDto userDto)
    {
        if (!Guid.TryParse(userDto.id, out Guid userId))
        {
            throw new Exception("Invalid User Id");
        }

        var user = await _userRepository.GetByIdAsync(userId);
        return await _userRepository.DeleteUser(user);
    }

    public async Task<User> UpdateUser(UserDto userDto)
    {
        var user = _mapper.Map<User>(userDto);

        //var user = await _userRepository.GetByIdAsync(userId);
        return await _userRepository.UpdateUser(user);
    }
}
