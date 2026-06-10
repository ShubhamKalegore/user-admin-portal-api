using CleanArchDemo.Application.DTOs;
using MediatR;

namespace CleanArchDemo.Application.Features.Users.Queries;

public sealed record GetUsersQuery : IRequest<List<UserResponseDto>>;
