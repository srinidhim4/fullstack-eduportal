using System.ComponentModel.DataAnnotations;

namespace EduPortal.API.Models;

/// <summary>
/// Course model — static data (could be DB-backed in a real app)
/// Week 2 Topic: Classes, Records, Generics
/// </summary>
public class Course
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public CourseLevel Level { get; set; }
    public string Icon { get; set; } = string.Empty;
}

/// <summary>
/// Enum for course levels — demonstrates Enums
/// Week 2 Topic: C# Programming Refresher
/// </summary>
public enum CourseLevel
{
    Beginner,
    Intermediate,
    Advanced
}

/// <summary>
/// Quiz submission result stored in database
/// Week 2 Topic: Entity Framework Core, Records
/// </summary>
public class QuizResult
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    [Required, MaxLength(50)]
    public string CourseId { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string CourseName { get; set; } = string.Empty;

    public int Score { get; set; }
    public int Total { get; set; }
    public double Percentage { get; set; }
    public bool Passed { get; set; }
    public DateTime TakenAt { get; set; } = DateTime.UtcNow;
}
