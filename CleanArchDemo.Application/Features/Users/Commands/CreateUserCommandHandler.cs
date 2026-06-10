using CleanArchDemo.Application.DTOs;
using CleanArchDemo.Application.Interfaces;
using CleanArchDemo.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CleanArchDemo.Application.Features.Users.Commands;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserResponseDto> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateUserCommand for email {Email}.", request.Email);

        if (await _userRepository.ExistsAsync(request.Email))
        {
            throw new InvalidOperationException("User already exists.");
        }

        var user = new User
        {
            Email = request.Email,
            Role = string.IsNullOrWhiteSpace(request.Role) ? "User" : request.Role
        };

        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);

        await _userRepository.AddAsync(user);

        return new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role
        };
    }
}
