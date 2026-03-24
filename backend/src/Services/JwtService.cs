using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using EduPortal.API.Models;

namespace EduPortal.API.Services;

/// <summary>
/// JWT Token generation service.
/// Week 2 Topic: Security, JWT tokens, Dependency Injection
/// 
/// Demonstrates:
///   - Generating signed JWT tokens
///   - Claims (sub, email, name)
///   - Token validation parameters
///   - IConfiguration usage
/// </summary>
public interface IJwtService
{
    string GenerateToken(Student student);
}

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;
    private readonly ILogger<JwtService> _logger;

    // Constructor injection — Week 2 Topic: Dependency Injection
    public JwtService(IConfiguration config, ILogger<JwtService> logger)
    {
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Generates a signed JWT token with student claims.
    /// Token expires in 7 days (configurable in appsettings.json)
    /// </summary>
    public string GenerateToken(Student student)
    {
        // Claims are pieces of information embedded in the token
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, student.Id.ToString()),
            new(ClaimTypes.Email,          student.Email),
            new(ClaimTypes.Name,           student.Name),
            new("student_id",              student.Id.ToString()),
        };

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:             _config["Jwt:Issuer"],
            audience:           _config["Jwt:Audience"],
            claims:             claims,
            expires:            DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        _logger.LogInformation("JWT token generated for student {Id}", student.Id);
        return tokenString;
    }
}
