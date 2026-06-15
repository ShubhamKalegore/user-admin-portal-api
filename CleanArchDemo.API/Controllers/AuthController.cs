using System.Security.Claims;
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
    public async Task<IActionResult> Login(UserDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (result == null)
            return BadRequest("Invalid credentials.");

        Response.Cookies.Append(
            "accessToken",
            result.AccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            });

        Response.Cookies.Append(
            "refreshToken",
            result.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(5)
            });

        return Ok(new
        {
            result.UserId
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            await _authService.LogoutAsync(userId);
        }

        Response.Cookies.Delete(
            "accessToken",
            new CookieOptions
            {
                Path = "/",
                Secure = true,
                SameSite = SameSiteMode.None
            });

        Response.Cookies.Delete(
            "refreshToken",
            new CookieOptions
            {
                Path = "/",
                Secure = true,
                SameSite = SameSiteMode.None
            });

        return Ok(new
        {
            Message = "Logged out successfully"
        });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken =
            Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized("Refresh token missing.");

        var result =
            await _authService.RefreshTokensAsync(refreshToken);

        if (result == null)
            return Unauthorized("Invalid token.");

        Response.Cookies.Append(
            "accessToken",
            result.AccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

        Response.Cookies.Append(
            "refreshToken",
            result.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

        return Ok();
    }
}