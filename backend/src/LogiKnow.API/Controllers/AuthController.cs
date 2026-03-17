using LogiKnow.Application.Common.DTOs;
using LogiKnow.Application.Interfaces;
using LogiKnow.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LogiKnow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<User> userManager, SignInManager<User> signInManager,
        IJwtService jwtService, ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct = default)
    {
        _logger.LogDebug("Register user: {Email}", request.Email);

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            PreferredLanguage = request.PreferredLanguage
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        await _userManager.AddToRoleAsync(user, "User");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        return Ok(new SingleResponse<AuthResponse>
        {
            Data = new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                UserId = user.Id,
                Email = user.Email,
                Roles = roles.ToList()
            }
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct = default)
    {
        _logger.LogDebug("Login user: {Email}", request.Email);

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Unauthorized(new { error = "Invalid email or password." });

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return Unauthorized(new { error = "Invalid email or password." });

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        return Ok(new SingleResponse<AuthResponse>
        {
            Data = new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                UserId = user.Id,
                Email = user.Email ?? "",
                Roles = roles.ToList()
            }
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct = default)
    {
        _logger.LogDebug("Refresh token");
        if (!_jwtService.ValidateRefreshToken(request.RefreshToken))
            return Unauthorized(new { error = "Invalid refresh token." });

        // In production, look up user by refresh token in DB
        return Ok(new { message = "Refresh token implementation requires token storage." });
    }
}
