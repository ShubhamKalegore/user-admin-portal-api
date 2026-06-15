using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CleanArchDemo.Application.DTOs;
using CleanArchDemo.Application.Interfaces;
using CleanArchDemo.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchDemo.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<User?> RegisterAsync(UserDto request)
    {
        _logger.LogInformation("Registration requested for email {Email}.", request.Email);

        if (await _userRepository.ExistsAsync(request.Email))
        {
            _logger.LogInformation("Registration failed because user already exists for email {Email}.", request.Email);
            return null;
        }

        var user = new User
        {
            Email = request.Email,
            Role = request.Role
        };
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);

        await _userRepository.AddAsync(user);
        _logger.LogInformation("User registered successfully with id {UserId}.", user.Id);

        return user;
    }

    public async Task<TokenResponseDto?> LoginAsync(UserDto request)
    {
        _logger.LogInformation("Login requested for email {Email}.", request.Email);

        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null)
        {
            _logger.LogInformation("Login failed because user was not found for email {Email}.", request.Email);
            return null;
        }

        var result = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            _logger.LogInformation("Login failed due to invalid credentials for user id {UserId}.", user.Id);
            return null;
        }

        var tokenResponse = await CreateTokenResponse(user);

        _logger.LogInformation("Login completed successfully for user id {UserId}.", user.Id);

        return tokenResponse;
    }

    //public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
    //{
    //    _logger.LogInformation("Refresh token requested for user id {UserId}.", request.UserId);

    //    var user = await _userRepository.GetByIdAsync(request.UserId);

    //    if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
    //    {
    //        _logger.LogInformation("Refresh token failed for user id {UserId}.", request.UserId);
    //        return null;
    //    }

    //    var tokenResponse = await CreateTokenResponse(user);
    //    _logger.LogInformation("Refresh token completed successfully for user id {UserId}.", user.Id);

    //    return tokenResponse;
    //}

    public async Task<TokenResponseDto?> RefreshTokensAsync(string refreshToken)
    {
        var user =
            await _userRepository
                .GetByRefreshTokenAsync(refreshToken);

        if (user is null)
            return null;

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return null;

        return await CreateTokenResponse(user);
    }
    private async Task<TokenResponseDto> CreateTokenResponse(User user)
    {
        var accessToken = CreateToken(user);
        _logger.LogInformation("Access token created for user id {UserId}.", user.Id);

        var refreshToken = await GenerateAndSaveRefreshTokenAsync(user);
        _logger.LogInformation("Refresh token created for user id {UserId}.", user.Id);

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.Id
        };
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
            audience: _configuration.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.Now.AddMinutes(5),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var refreshToken = Convert.ToBase64String(randomNumber);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(5);
        await _userRepository.UpdateAsync(user);
        _logger.LogInformation(
            "Refresh token saved for user id {UserId} with expiry {RefreshTokenExpiryTime}.",
            user.Id,
            user.RefreshTokenExpiryTime);

        return refreshToken;
    }

    public async Task LogoutAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
            return;

        user.RefreshToken = string.Empty;
        user.RefreshTokenExpiryTime = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
    }
}
