using CleanArchDemo.Application.DTOs;
using CleanArchDemo.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchDemo.Application.Features.Users.Queries;

public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUsersQueryHandler> _logger;

    public GetUsersQueryHandler(
        IUserRepository userRepository,
        ILogger<GetUsersQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<List<UserResponseDto>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetUsersQuery.");

        var users = await _userRepository.GetAllAsync();

        return users.Select(user => new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role
        }).ToList();
    }
}
