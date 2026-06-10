using CleanArchDemo.Application.DTOs;
using MediatR;

namespace CleanArchDemo.Application.Features.Users.Commands;

public sealed record CreateUserCommand(
    string Email,
    string Password,
    string Role = "User") : IRequest<UserResponseDto>;
