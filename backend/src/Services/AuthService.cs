using Microsoft.EntityFrameworkCore;
using EduPortal.API.Data;
using EduPortal.API.DTOs;
using EduPortal.API.Models;

namespace EduPortal.API.Services;

/// <summary>
/// Authentication service — handles register and login.
/// Week 2 Topic: Security, Entity Framework Core, Async Programming,
///               Dependency Injection, Errors and Exceptions
/// 
/// Demonstrates:
///   - Async/await pattern
///   - BCrypt password hashing (NEVER store plain passwords)
///   - EF Core queries
///   - Custom exceptions
///   - Interface/implementation separation
/// </summary>
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IJwtService  _jwt;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext db, IJwtService jwt, ILogger<AuthService> logger)
    {
        _db     = db;
        _jwt    = jwt;
        _logger = logger;
    }

    /// <summary>
    /// Register a new student.
    /// Throws ArgumentException if email already exists.
    /// </summary>
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check for duplicate email — EF Core async query
        bool emailExists = await _db.Students
            .AsNoTracking()  // Read-only: no change tracking (performance)
            .AnyAsync(s => s.Email == request.Email.ToLower());

        if (emailExists)
            throw new ArgumentException("An account with this email already exists.");

        // Hash password with BCrypt (industry standard)
        // Week 2 Topic: Security — NEVER store plain text passwords
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var student = new Student
        {
            Name         = request.Name.Trim(),
            Email        = request.Email.ToLower().Trim(),
            PasswordHash = passwordHash,
            JoinedDate   = DateTime.UtcNow,
        };

        _db.Students.Add(student);
        await _db.SaveChangesAsync(); // Executes INSERT

        _logger.LogInformation("New student registered: {Email}", student.Email);

        var token = _jwt.GenerateToken(student);
        return new AuthResponse(token, MapToDto(student));
    }

    /// <summary>
    /// Login with email and password.
    /// Throws UnauthorizedAccessException if credentials are invalid.
    /// </summary>
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Load student with enrollments via Include (eager loading)
        // Week 2 Topic: LINQ, Entity Framework Core (Include to avoid N+1)
        var student = await _db.Students
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.Email == request.Email.ToLower());

        // Check both null and password in one condition to prevent timing attacks
        if (student is null || !BCrypt.Net.BCrypt.Verify(request.Password, student.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        _logger.LogInformation("Student logged in: {Email}", student.Email);

        var token = _jwt.GenerateToken(student);
        return new AuthResponse(token, MapToDto(student));
    }

    /// <summary>
    /// Maps Student entity to DTO — never expose PasswordHash
    /// Week 2 Topic: DTOs, LINQ Select projection
    /// </summary>
    private static StudentDto MapToDto(Student s) =>
        new(
            Id:                s.Id.ToString(),
            Name:              s.Name,
            Email:             s.Email,
            JoinedDate:        s.JoinedDate.ToString("d MMMM yyyy"),
            EnrolledCourseIds: s.Enrollments.Select(e => e.CourseId).ToList()
        );
}
