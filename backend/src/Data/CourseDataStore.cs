using EduPortal.API.Models;

namespace EduPortal.API.Data;

/// <summary>
/// In-memory static course data store.
/// In a real app this would be a database table.
/// Week 2 Topic: Collections, LINQ
/// </summary>
public static class CourseDataStore
{
    // Using IReadOnlyList to prevent external modification (immutable collection)
    public static IReadOnlyList<Course> Courses { get; } = new List<Course>
    {
        new() {
            Id = "c1", Icon = "💻",
            Name = "C# Programming Fundamentals",
            Instructor = "Dr. Ananya Sharma",
            Description = "Master the basics of C# — variables, OOP, collections, LINQ, async programming and more.",
            Category = "Backend", Duration = "3 hrs", Level = CourseLevel.Beginner
        },
        new() {
            Id = "c2", Icon = "⚙️",
            Name = "Essentials of .NET",
            Instructor = "Prof. Rajan Mehta",
            Description = "Deep dive into .NET ecosystem: EF Core, Design Patterns, Unit Testing, and Swagger.",
            Category = "Backend", Duration = "10 hrs", Level = CourseLevel.Intermediate
        },
        new() {
            Id = "c3", Icon = "🌐",
            Name = "ASP.NET Core Web APIs",
            Instructor = "Ms. Priya Nair",
            Description = "Build production-grade RESTful APIs with ASP.NET Core, middleware, JWT auth, and TDD.",
            Category = "Web", Duration = "9 hrs", Level = CourseLevel.Intermediate
        },
        new() {
            Id = "c4", Icon = "🚀",
            Name = "Advanced Backend Concepts",
            Instructor = "Mr. Arjun Patel",
            Description = "SignalR, Microservices, Docker, OAuth2, caching strategies, and API versioning.",
            Category = "Advanced", Duration = "15 hrs", Level = CourseLevel.Advanced
        },
        new() {
            Id = "c5", Icon = "🗄️",
            Name = "Entity Framework Core",
            Instructor = "Dr. Kavitha Rao",
            Description = "ORM mastery: Code-First, migrations, LINQ queries, relationships and performance tuning.",
            Category = "Data", Duration = "6 hrs", Level = CourseLevel.Intermediate
        },
        new() {
            Id = "c6", Icon = "🐳",
            Name = "Docker & Containerization",
            Instructor = "Mr. Vikram Singh",
            Description = "Containerize .NET apps with Docker, multi-stage builds, Docker Compose and Kubernetes basics.",
            Category = "DevOps", Duration = "4 hrs", Level = CourseLevel.Intermediate
        },
    };

    /// <summary>
    /// Find a course by ID — demonstrates LINQ extension methods
    /// Week 2 Topic: LINQ, Extension Methods
    /// </summary>
    public static Course? FindById(string id) =>
        Courses.FirstOrDefault(c => c.Id == id);

    /// <summary>
    /// Filter courses by level — demonstrates lambda expressions
    /// Week 2 Topic: Delegates, Lambdas, LINQ
    /// </summary>
    public static IEnumerable<Course> ByLevel(CourseLevel level) =>
        Courses.Where(c => c.Level == level);

    /// <summary>
    /// Filter by category — demonstrates LINQ chaining
    /// </summary>
    public static IEnumerable<Course> ByCategory(string category) =>
        Courses.Where(c => c.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
}
