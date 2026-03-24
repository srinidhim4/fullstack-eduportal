using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EduPortal.API.DTOs;
using EduPortal.API.Services;

namespace EduPortal.API.Controllers;

/// <summary>
/// Quiz controller — submit and retrieve quiz results.
/// Week 2 Topic: ASP.NET Core, EF Core, LINQ, Async Programming
/// </summary>
[ApiController]
[Route("api/quiz")]
[Authorize]
public class QuizController : ControllerBase
{
    private readonly IStudentService _studentService;

    public QuizController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>
    /// Submit a quiz result.
    /// POST /api/quiz/{courseId}/submit
    /// </summary>
    [HttpPost("{courseId}/submit")]
    [ProducesResponseType(typeof(QuizResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Submit(string courseId, [FromBody] QuizSubmitRequest request)
    {
        // Ensure the courseId in the URL matches the body
        if (courseId != request.CourseId)
            return BadRequest(new { message = "Course ID mismatch." });

        var studentId = GetStudentId();
        var result = await _studentService.SaveQuizResultAsync(studentId, request);
        return Ok(result);
    }

    /// <summary>
    /// Get all quiz results for the current student.
    /// GET /api/quiz/results
    /// </summary>
    [HttpGet("results")]
    [ProducesResponseType(typeof(IEnumerable<QuizResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetResults()
    {
        var studentId = GetStudentId();
        var results = await _studentService.GetQuizResultsAsync(studentId);
        return Ok(results);
    }

    private int GetStudentId()
    {
        var claim = User.FindFirst("student_id")?.Value
                 ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? throw new UnauthorizedAccessException("Invalid token.");
        return int.Parse(claim);
    }
}
