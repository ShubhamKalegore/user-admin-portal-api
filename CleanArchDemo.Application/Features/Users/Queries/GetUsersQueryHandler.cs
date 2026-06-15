using CleanArchDemo.Application.DTOs;
using CleanArchDemo.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace CleanArchDemo.Application.Features.Users.Queries;

public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUsersQueryHandler> _logger;
    private readonly IMemoryCache _cache;

    public GetUsersQueryHandler(
        IUserRepository userRepository,
        ILogger<GetUsersQueryHandler> logger,
        IMemoryCache cache)
    {
        _userRepository = userRepository;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<UserResponseDto>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetUsersQuery.");

        if (_cache.TryGetValue("users", out List<UserResponseDto>? cachedUsers))
        {
            _logger.LogInformation("Users fetched from cache.");
            return cachedUsers!;
        }

        _logger.LogInformation("Users loaded from database.");

        var users = await _userRepository.GetAllAsync();

        var result = users.Select(user => new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role
        }).ToList();

        _cache.Set(
            "users",
            result,
            TimeSpan.FromMinutes(5));

        return result;
    }
}
