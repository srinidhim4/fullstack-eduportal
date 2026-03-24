using System.ComponentModel.DataAnnotations;

namespace EduPortal.API.Models;

/// <summary>
/// Represents a registered student in the system.
/// Demonstrates: Classes, Properties, Data Annotations, Nullable types
/// Week 2 Topic: Classes, Records, Structs — Object-Oriented Programming
/// </summary>
public class Student
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password stored as BCrypt hash — NEVER store plain text
    /// Week 2 Topic: Security
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

    // Navigation property — EF Core relationship
    // Week 2 Topic: Entity Framework Core
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}

/// <summary>
/// Enrollment join entity — many-to-many between Student and Course
/// </summary>
public class Enrollment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
    public string CourseId { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
}
