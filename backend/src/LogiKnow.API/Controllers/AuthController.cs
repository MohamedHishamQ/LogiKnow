using LogiKnow.Application.Common.DTOs;
using LogiKnow.Application.Interfaces;
using LogiKnow.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LogiKnow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthController> _logger;

    private const string AccessTokenCookie = "access_token";
    private const string RefreshTokenCookie = "refresh_token";

    public AuthController(
        UserManager<User> userManager, SignInManager<User> signInManager,
        IJwtService jwtService, IEmailService emailService, ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _emailService = emailService;
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

        // Store refresh token hash in DB
        await StoreRefreshTokenAsync(user, refreshToken);

        // Set httpOnly cookies
        SetAuthCookies(token, refreshToken);

        // Send welcome email (fire-and-forget)
        _ = Task.Run(() => _emailService.SendWelcomeEmailAsync(
            user.Email!, user.FullName ?? user.Email!, ct), ct);

        return Ok(new SingleResponse<AuthResponse>
        {
            Data = new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
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

        // Store refresh token hash in DB
        await StoreRefreshTokenAsync(user, refreshToken);

        // Set httpOnly cookies
        SetAuthCookies(token, refreshToken);

        return Ok(new SingleResponse<AuthResponse>
        {
            Data = new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                UserId = user.Id,
                Email = user.Email ?? "",
                FullName = user.FullName,
                Roles = roles.ToList()
            }
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken ct = default)
    {
        _logger.LogDebug("Refresh token");

        // Read refresh token from cookie
        var refreshToken = Request.Cookies[RefreshTokenCookie];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(new { error = "No refresh token provided." });

        // Hash the incoming token and find user with matching hash
        var hash = HashToken(refreshToken);
        var users = _userManager.Users.Where(u =>
            u.RefreshTokenHash == hash &&
            u.RefreshTokenExpiry > DateTime.UtcNow);

        var user = users.FirstOrDefault();
        if (user is null)
            return Unauthorized(new { error = "Invalid or expired refresh token." });

        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _jwtService.GenerateToken(user, roles);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        await StoreRefreshTokenAsync(user, newRefreshToken);
        SetAuthCookies(newAccessToken, newRefreshToken);

        return Ok(new SingleResponse<AuthResponse>
        {
            Data = new AuthResponse
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                UserId = user.Id,
                Email = user.Email ?? "",
                FullName = user.FullName,
                Roles = roles.ToList()
            }
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken ct = default)
    {
        _logger.LogDebug("Logout user");

        // Clear refresh token from DB
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.RefreshTokenHash = null;
                user.RefreshTokenExpiry = null;
                await _userManager.UpdateAsync(user);
            }
        }

        // Clear cookies
        Response.Cookies.Delete(AccessTokenCookie);
        Response.Cookies.Delete(RefreshTokenCookie);

        return Ok(new { message = "Logged out successfully." });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            userId = user.Id,
            email = user.Email,
            fullName = user.FullName,
            preferredLanguage = user.PreferredLanguage,
            roles = roles.ToList(),
            createdAt = user.CreatedAt
        });
    }

    // ===== Helpers =====

    private async Task StoreRefreshTokenAsync(User user, string refreshToken)
    {
        user.RefreshTokenHash = HashToken(refreshToken);
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);
    }

    private void SetAuthCookies(string accessToken, string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        };

        Response.Cookies.Append(AccessTokenCookie, accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddMinutes(60)
        });

        Response.Cookies.Append(RefreshTokenCookie, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/api/auth",
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}
