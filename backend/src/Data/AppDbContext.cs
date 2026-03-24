using Microsoft.EntityFrameworkCore;
using EduPortal.API.Models;

namespace EduPortal.API.Data;

/// <summary>
/// Entity Framework Core DbContext
/// Week 2 Topic: Entity Framework Core, Dependency Injection, Configuration
/// 
/// Demonstrates:
///   - DbContext setup with SQL Server
///   - DbSet declarations (maps to tables)
///   - Fluent API configuration in OnModelCreating
///   - Relationships (One-to-Many)
///   - Data seeding
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSet = table in the database
    public DbSet<Student>    Students    => Set<Student>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<QuizResult> QuizResults => Set<QuizResult>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        // ── Student configuration ────────────────────────────────────
        mb.Entity<Student>(e =>
        {
            e.HasIndex(s => s.Email).IsUnique(); // Unique email constraint
            e.Property(s => s.Name).HasMaxLength(100).IsRequired();
            e.Property(s => s.Email).HasMaxLength(200).IsRequired();
            e.Property(s => s.JoinedDate).HasDefaultValueSql("GETUTCDATE()");
        });

        // ── Enrollment (join table) ──────────────────────────────────
        mb.Entity<Enrollment>(e =>
        {
            // Unique constraint: one enrollment per student per course
            e.HasIndex(en => new { en.StudentId, en.CourseId }).IsUnique();

            // Relationship: Student has many Enrollments
            e.HasOne(en => en.Student)
             .WithMany(s => s.Enrollments)
             .HasForeignKey(en => en.StudentId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── QuizResult ───────────────────────────────────────────────
        mb.Entity<QuizResult>(e =>
        {
            e.HasOne(r => r.Student)
             .WithMany()
             .HasForeignKey(r => r.StudentId)
             .OnDelete(DeleteBehavior.Cascade);

            e.Property(r => r.Percentage).HasColumnType("decimal(5,2)");
        });
    }
}
