using CleanArchDemo.Application.DTOs;
using CleanArchDemo.Domain.Entities;

namespace CleanArchDemo.Application.Interfaces;

public interface IUserService
{
    Task<List<UserResponseDto>> GetAllUsersAsync();
    Task<bool> DeleteUser(UserDto userDto);
    Task<User> UpdateUser(UserDto userDto);
}
