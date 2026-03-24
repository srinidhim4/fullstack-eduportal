using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EduPortal.API.Data;
using EduPortal.API.DTOs;
using EduPortal.API.Services;

namespace EduPortal.API.Controllers;

/// <summary>
/// Courses controller — list courses and enroll.
/// Week 2 Topic: ASP.NET Core, Authorization, LINQ, RESTful APIs
/// </summary>
[ApiController]
[Route("api/courses")]
[Authorize] // All endpoints require a valid JWT token
public class CoursesController : ControllerBase
{
    private readonly IStudentService _studentService;

    public CoursesController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>
    /// Get all available courses.
    /// GET /api/courses
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<CourseDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        // LINQ projection — map Course to CourseDto
        var dtos = CourseDataStore.Courses.Select(c => new CourseDto(
            Id:          c.Id,
            Name:        c.Name,
            Instructor:  c.Instructor,
            Description: c.Description,
            Category:    c.Category,
            Duration:    c.Duration,
            Level:       c.Level.ToString(),
            Icon:        c.Icon
        ));
        return Ok(dtos);
    }

    /// <summary>
    /// Get a single course by ID.
    /// GET /api/courses/{id}
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(string id)
    {
        var course = CourseDataStore.FindById(id);
        if (course is null) return NotFound(new { message = $"Course '{id}' not found." });

        return Ok(new CourseDto(
            Id: course.Id, Name: course.Name, Instructor: course.Instructor,
            Description: course.Description, Category: course.Category,
            Duration: course.Duration, Level: course.Level.ToString(), Icon: course.Icon
        ));
    }

    /// <summary>
    /// Enroll the current student in a course.
    /// POST /api/courses/{id}/enroll
    /// </summary>
    [HttpPost("{id}/enroll")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Enroll(string id)
    {
        var studentId = GetStudentId();
        var student = await _studentService.EnrollAsync(studentId, id);
        return Ok(student);
    }

    // ─── Helper: extract student ID from JWT claims ───────────────────────────
    private int GetStudentId()
    {
        var claim = User.FindFirst("student_id")?.Value
                 ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? throw new UnauthorizedAccessException("Invalid token.");
        return int.Parse(claim);
    }
}
