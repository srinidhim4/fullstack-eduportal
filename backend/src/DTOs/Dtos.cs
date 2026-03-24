using System.ComponentModel.DataAnnotations;

namespace EduPortal.API.DTOs;

// ─── Auth DTOs ─────────────────────────────────────────────────────────────────
// Week 2 Topic: Records (immutable DTOs), Validation

/// <summary>Login request DTO</summary>
public record LoginRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required] string Password
);

/// <summary>Register request DTO</summary>
public record RegisterRequest(
    [property: Required, MaxLength(100)] string Name,
    [property: Required, EmailAddress, MaxLength(200)] string Email,
    [property: Required, MinLength(6)] string Password
);

/// <summary>Auth response — returned on login/register</summary>
public record AuthResponse(
    string Token,
    StudentDto Student
);

// ─── Student DTOs ──────────────────────────────────────────────────────────────

/// <summary>
/// Student DTO — never expose PasswordHash to client!
/// Week 2 Topic: Security, DTOs
/// </summary>
public record StudentDto(
    string Id,
    string Name,
    string Email,
    string JoinedDate,
    List<string> EnrolledCourseIds
);

/// <summary>Update profile request</summary>
public record UpdateProfileRequest(
    [property: Required, MaxLength(100)] string Name
);

// ─── Course DTOs ──────────────────────────────────────────────────────────────

public record CourseDto(
    string Id,
    string Name,
    string Instructor,
    string Description,
    string Category,
    string Duration,
    string Level,
    string Icon
);

// ─── Quiz DTOs ────────────────────────────────────────────────────────────────

/// <summary>Quiz result submission from frontend</summary>
public record QuizSubmitRequest(
    [property: Required] string CourseId,
    [property: Required] string CourseName,
    int Score,
    int Total,
    double Percentage,
    bool Passed
);

/// <summary>Quiz result response</summary>
public record QuizResultDto(
    int Id,
    string CourseId,
    string CourseName,
    int Score,
    int Total,
    double Percentage,
    bool Passed,
    string TakenAt
);

// ─── Common ───────────────────────────────────────────────────────────────────

/// <summary>
/// Generic API response wrapper — demonstrates Generics
/// Week 2 Topic: Generics
/// </summary>
public record ApiResponse<T>(
    bool Success,
    T? Data,
    string? Message = null
);

/// <summary>
/// Standard error response following RFC 7807
/// Week 2 Topic: Errors and Exceptions
/// </summary>
public record ErrorResponse(
    string Message,
    int StatusCode,
    string? Details = null
);
