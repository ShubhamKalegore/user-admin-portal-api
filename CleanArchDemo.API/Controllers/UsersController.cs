using CleanArchDemo.Application.DTOs;
using CleanArchDemo.Application.Features.Users.Commands;
using CleanArchDemo.Application.Features.Users.Queries;
using CleanArchDemo.Application.Interfaces;
using CleanArchDemo.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchDemo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Admin")]
//[Authorize]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IUserService _userService;

    public UsersController(ISender sender, IUserService userService)
    {
        _sender = sender;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _sender.Send(new GetUsersQuery());
        return Ok(users);
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponseDto>> GetById(Guid id)
    {
        var user = await _sender.Send(new GetUserByIdQuery(id));
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> Create(UserDto request)
    {
        try
        {
            var user = await _sender.Send(new CreateUserCommand(
                request.Email,
                request.Password,
                request.Role));

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete]
    public async Task<ActionResult<UserResponseDto>> Delete(UserDto request)
    {
        try
        {
            var result = await _userService.DeleteUser(request);

            if (!result)
            {
                throw new Exception("User could not be deleted.");
            }

            return Ok();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }
    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<ActionResult<UserResponseDto>> Update(UserDto request)
    {
        try
        {
            var result = await _userService.UpdateUser(request);

            return Ok(result);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}
