using Microsoft.AspNetCore.Mvc;
using EduPortal.API.DTOs;
using EduPortal.API.Services;

namespace EduPortal.API.Controllers;

/// <summary>
/// Authentication controller — register and login endpoints.
/// Week 2 Topic: ASP.NET Core, RESTful APIs, JWT, Dependency Injection
/// 
/// Demonstrates:
///   - [ApiController] with automatic model validation
///   - Route attributes
///   - Async controller actions
///   - Constructor injection
///   - HTTP verb attributes
///   - Proper status codes (201 Created, 200 OK, 400 Bad Request)
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService auth, ILogger<AuthController> logger)
    {
        _auth   = auth;
        _logger = logger;
    }

    /// <summary>
    /// Register a new student account.
    /// POST /api/auth/register
    /// </summary>
    /// <returns>JWT token + student profile</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // [ApiController] automatically validates ModelState — no need to check manually
        var response = await _auth.RegisterAsync(request);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// Login with email and password.
    /// POST /api/auth/login
    /// </summary>
    /// <returns>JWT token + student profile</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _auth.LoginAsync(request);
        return Ok(response);
    }
}
