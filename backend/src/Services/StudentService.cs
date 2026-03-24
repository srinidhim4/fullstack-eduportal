using Microsoft.EntityFrameworkCore;
using EduPortal.API.Data;
using EduPortal.API.DTOs;
using EduPortal.API.Models;

namespace EduPortal.API.Services;

/// <summary>
/// Student service — enrollment, profile, quiz results.
/// Week 2 Topic: EF Core, LINQ, Async, Generics, Collections
/// </summary>
public interface IStudentService
{
    Task<StudentDto?> GetByIdAsync(int studentId);
    Task<StudentDto>  UpdateProfileAsync(int studentId, UpdateProfileRequest request);
    Task<StudentDto>  EnrollAsync(int studentId, string courseId);
    Task<QuizResultDto> SaveQuizResultAsync(int studentId, QuizSubmitRequest request);
    Task<IEnumerable<QuizResultDto>> GetQuizResultsAsync(int studentId);
}

public class StudentService : IStudentService
{
    private readonly AppDbContext _db;
    private readonly ILogger<StudentService> _logger;

    public StudentService(AppDbContext db, ILogger<StudentService> logger)
    {
        _db     = db;
        _logger = logger;
    }

    public async Task<StudentDto?> GetByIdAsync(int studentId)
    {
        var student = await _db.Students
            .Include(s => s.Enrollments)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == studentId);

        return student is null ? null : MapToDto(student);
    }

    /// <summary>
    /// Update student's display name.
    /// Throws KeyNotFoundException if student doesn't exist.
    /// </summary>
    public async Task<StudentDto> UpdateProfileAsync(int studentId, UpdateProfileRequest request)
    {
        var student = await _db.Students
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.Id == studentId)
            ?? throw new KeyNotFoundException("Student not found.");

        student.Name = request.Name.Trim();
        await _db.SaveChangesAsync();

        _logger.LogInformation("Profile updated for student {Id}", studentId);
        return MapToDto(student);
    }

    /// <summary>
    /// Enroll student in a course.
    /// Idempotent — enrolling twice does nothing.
    /// Week 2 Topic: EF Core, Transactions, LINQ
    /// </summary>
    public async Task<StudentDto> EnrollAsync(int studentId, string courseId)
    {
        // Validate course exists
        if (CourseDataStore.FindById(courseId) is null)
            throw new ArgumentException($"Course '{courseId}' not found.");

        var student = await _db.Students
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.Id == studentId)
            ?? throw new KeyNotFoundException("Student not found.");

        // Check if already enrolled — idempotent operation
        bool alreadyEnrolled = student.Enrollments.Any(e => e.CourseId == courseId);
        if (!alreadyEnrolled)
        {
            student.Enrollments.Add(new Enrollment
            {
                StudentId  = studentId,
                CourseId   = courseId,
                EnrolledAt = DateTime.UtcNow,
            });
            await _db.SaveChangesAsync();
            _logger.LogInformation("Student {Id} enrolled in course {CourseId}", studentId, courseId);
        }

        return MapToDto(student);
    }

    /// <summary>
    /// Save or update a quiz result.
    /// Week 2 Topic: EF Core upsert pattern
    /// </summary>
    public async Task<QuizResultDto> SaveQuizResultAsync(int studentId, QuizSubmitRequest request)
    {
        // Upsert: update existing result or create new one
        var existing = await _db.QuizResults
            .FirstOrDefaultAsync(r => r.StudentId == studentId && r.CourseId == request.CourseId);

        if (existing is not null)
        {
            // Update
            existing.Score      = request.Score;
            existing.Total      = request.Total;
            existing.Percentage = request.Percentage;
            existing.Passed     = request.Passed;
            existing.TakenAt    = DateTime.UtcNow;
        }
        else
        {
            // Insert
            existing = new QuizResult
            {
                StudentId  = studentId,
                CourseId   = request.CourseId,
                CourseName = request.CourseName,
                Score      = request.Score,
                Total      = request.Total,
                Percentage = request.Percentage,
                Passed     = request.Passed,
                TakenAt    = DateTime.UtcNow,
            };
            _db.QuizResults.Add(existing);
        }

        await _db.SaveChangesAsync();
        return MapResultToDto(existing);
    }

    /// <summary>
    /// Get all quiz results for a student.
    /// Week 2 Topic: LINQ, Async Enumerable
    /// </summary>
    public async Task<IEnumerable<QuizResultDto>> GetQuizResultsAsync(int studentId)
    {
        // LINQ query on IQueryable — executed as SQL
        var results = await _db.QuizResults
            .Where(r => r.StudentId == studentId)
            .OrderByDescending(r => r.TakenAt)
            .AsNoTracking()
            .ToListAsync();

        // LINQ Select projection to DTO
        return results.Select(MapResultToDto);
    }

    // ─── Private helpers (static — no instance state needed) ─────────────────

    private static StudentDto MapToDto(Student s) =>
        new(
            Id:                s.Id.ToString(),
            Name:              s.Name,
            Email:             s.Email,
            JoinedDate:        s.JoinedDate.ToString("d MMMM yyyy"),
            EnrolledCourseIds: s.Enrollments.Select(e => e.CourseId).ToList()
        );

    private static QuizResultDto MapResultToDto(QuizResult r) =>
        new(
            Id:         r.Id,
            CourseId:   r.CourseId,
            CourseName: r.CourseName,
            Score:      r.Score,
            Total:      r.Total,
            Percentage: r.Percentage,
            Passed:     r.Passed,
            TakenAt:    r.TakenAt.ToString("d MMMM yyyy")
        );
}
