using CleanArchDemo.Application.DTOs;
using CleanArchDemo.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(UserDto request)
    {
        var user = await _authService.RegisterAsync(request);
        if (user == null) return BadRequest("User exists.");
        return Ok("Success");
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
    {
        var result = await _authService.LoginAsync(request);
        if (result == null) return BadRequest("Invalid credentials.");
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenResponseDto>> Refresh(RefreshTokenRequestDto request)
    {
        var result = await _authService.RefreshTokensAsync(request);
        if (result == null) return Unauthorized("Invalid token.");
        return Ok(result);
    }
}