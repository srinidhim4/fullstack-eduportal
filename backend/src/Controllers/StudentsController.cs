using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EduPortal.API.DTOs;
using EduPortal.API.Services;

namespace EduPortal.API.Controllers;

/// <summary>
/// Students controller — profile management.
/// Week 2 Topic: ASP.NET Core, Authorization, Async Programming
/// </summary>
[ApiController]
[Route("api/students")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>
    /// Get the currently logged-in student's profile.
    /// GET /api/students/me
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMe()
    {
        var studentId = GetStudentId();
        var student = await _studentService.GetByIdAsync(studentId);
        return student is null ? NotFound() : Ok(student);
    }

    /// <summary>
    /// Update the current student's display name.
    /// PUT /api/students/profile
    /// </summary>
    [HttpPut("profile")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var studentId = GetStudentId();
        var updated = await _studentService.UpdateProfileAsync(studentId, request);
        return Ok(updated);
    }

    private int GetStudentId()
    {
        var claim = User.FindFirst("student_id")?.Value
                 ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? throw new UnauthorizedAccessException("Invalid token.");
        return int.Parse(claim);
    }
}
