using CleanArchDemo.Application.DTOs;

namespace CleanArchDemo.Application.Interfaces;

public interface IUserService
{
    Task<List<UserResponseDto>> GetAllUsersAsync();
}
