using CleanArchDemo.Application.DTOs;
using CleanArchDemo.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchDemo.Application.Features.Users.Queries;

public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResponseDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserResponseDto?> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetUserByIdQuery for user id {UserId}.", request.Id);

        var user = await _userRepository.GetByIdAsync(request.Id);
        if (user is null)
        {
            return null;
        }

        return new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role
        };
    }
}
