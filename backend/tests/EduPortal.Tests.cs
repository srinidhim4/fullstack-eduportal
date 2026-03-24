using System;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using EduPortal.API.DTOs;
using EduPortal.API.Models;
using EduPortal.API.Services;
using System.Collections.Generic;
using System.Linq;

namespace EduPortal.Tests;

/**
 * Unit Tests for AuthService and StudentService
 * Week 2 Topic: Unit Testing, TDD, Unit Testing Frameworks (xUnit + Moq)
 *
 * Demonstrates:
 *   - [Fact] for single test cases
 *   - [Theory] + [InlineData] for parameterized tests
 *   - Mock<T> to isolate dependencies
 *   - AAA pattern: Arrange → Act → Assert
 *   - Testing exception paths
 */

// ── AuthService Tests ─────────────────────────────────────────────────────────

/// <summary>
/// Tests for the JWT token service.
/// Uses a mock IConfiguration to avoid needing real config files.
/// </summary>
public class JwtServiceTests
{
    [Fact]
    public void GenerateToken_ShouldReturnNonEmptyString_ForValidStudent()
    {
        // Arrange
        var configMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        configMock.Setup(c => c["Jwt:Key"]).Returns("TestSecretKey12345678901234567890"); // 32+ chars
        configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        var loggerMock = new Mock<ILogger<JwtService>>();
        var service = new JwtService(configMock.Object, loggerMock.Object);

        var student = new Student
        {
            Id    = 1,
            Name  = "Alice Smith",
            Email = "alice@example.com",
        };

        // Act
        var token = service.GenerateToken(student);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
        Assert.Contains(".", token); // JWT has 3 dot-separated parts
    }

    [Fact]
    public void GenerateToken_ShouldContainThreeParts_WhenValid()
    {
        // Arrange
        var configMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        configMock.Setup(c => c["Jwt:Key"]).Returns("TestSecretKey12345678901234567890");
        configMock.Setup(c => c["Jwt:Issuer"]).Returns("EduPortal");
        configMock.Setup(c => c["Jwt:Audience"]).Returns("EduPortalUsers");

        var service = new JwtService(configMock.Object, new Mock<ILogger<JwtService>>().Object);
        var student = new Student { Id = 2, Name = "Bob", Email = "bob@test.com" };

        // Act
        var token = service.GenerateToken(student);

        // Assert — JWT format is always header.payload.signature
        var parts = token.Split('.');
        Assert.Equal(3, parts.Length);
    }
}

// ── Validation / DTO Tests ────────────────────────────────────────────────────

/// <summary>
/// Tests for request validation logic.
/// Demonstrates parameterized [Theory] tests.
/// </summary>
public class LoginRequestValidationTests
{
    [Theory]
    [InlineData("", "password123")]        // empty email
    [InlineData("notanemail", "pass")]     // invalid email format
    [InlineData("test@test.com", "")]      // empty password
    public void LoginRequest_ShouldBeInvalid_ForBadInputs(string email, string password)
    {
        // Arrange
        var request = new LoginRequest(email, password);

        // Act — validate using DataAnnotations
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(request);
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            request, context, results, validateAllProperties: true);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(results);
    }

    [Fact]
    public void LoginRequest_ShouldBeValid_WithCorrectInputs()
    {
        // Arrange
        var request = new LoginRequest("alice@example.com", "mypassword");

        // Act
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(request);
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            request, context, results, validateAllProperties: true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }
}

// ── Password Hashing Tests ────────────────────────────────────────────────────

/// <summary>
/// Demonstrates password hashing security concepts.
/// Week 2 Topic: Security
/// </summary>
public class PasswordHashingTests
{
    [Fact]
    public void BCrypt_ShouldHash_AndVerifyPassword()
    {
        // Arrange
        const string plainPassword = "MySecureP@ss123";

        // Act
        string hash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        bool isValid = BCrypt.Net.BCrypt.Verify(plainPassword, hash);

        // Assert
        Assert.True(isValid);
        Assert.NotEqual(plainPassword, hash); // Never store plain text
    }

    [Fact]
    public void BCrypt_ShouldRejectWrongPassword()
    {
        // Arrange
        string hash = BCrypt.Net.BCrypt.HashPassword("correct-password");

        // Act
        bool isValid = BCrypt.Net.BCrypt.Verify("wrong-password", hash);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void BCrypt_TwoHashesOfSamePassword_ShouldBeDifferent()
    {
        // BCrypt uses a random salt — same password produces different hashes
        string hash1 = BCrypt.Net.BCrypt.HashPassword("samepassword");
        string hash2 = BCrypt.Net.BCrypt.HashPassword("samepassword");

        Assert.NotEqual(hash1, hash2);                                // Different hashes
        Assert.True(BCrypt.Net.BCrypt.Verify("samepassword", hash1)); // Both verify
        Assert.True(BCrypt.Net.BCrypt.Verify("samepassword", hash2));
    }
}

// ── Course Data Tests ─────────────────────────────────────────────────────────

/// <summary>
/// Tests for CourseDataStore static data and LINQ queries.
/// Week 2 Topic: LINQ, Collections
/// </summary>
public class CourseDataStoreTests
{
    [Fact]
    public void Courses_ShouldHaveAtLeastOneCourse()
    {
        Assert.NotEmpty(EduPortal.API.Data.CourseDataStore.Courses);
    }

    [Theory]
    [InlineData("c1")]
    [InlineData("c2")]
    [InlineData("c3")]
    public void FindById_ShouldReturnCourse_ForValidId(string courseId)
    {
        var course = EduPortal.API.Data.CourseDataStore.FindById(courseId);
        Assert.NotNull(course);
        Assert.Equal(courseId, course.Id);
    }

    [Fact]
    public void FindById_ShouldReturnNull_ForInvalidId()
    {
        var course = EduPortal.API.Data.CourseDataStore.FindById("invalid-id");
        Assert.Null(course);
    }

    [Fact]
    public void AllCourses_ShouldHaveUniqueIds()
    {
        var courses = EduPortal.API.Data.CourseDataStore.Courses;
        var uniqueIds = courses.Select(c => c.Id).Distinct().Count();
        Assert.Equal(courses.Count, uniqueIds);
    }

    [Fact]
    public void ByCategory_ShouldBeCaseInsensitive()
    {
        var lower = EduPortal.API.Data.CourseDataStore.ByCategory("backend");
        var upper = EduPortal.API.Data.CourseDataStore.ByCategory("BACKEND");
        Assert.Equal(lower.Count(), upper.Count());
    }
}

// ── Quiz Result Tests ─────────────────────────────────────────────────────────

/// <summary>
/// Tests for quiz pass/fail logic.
/// Week 2 Topic: Unit Testing, Business Logic
/// </summary>
public class QuizLogicTests
{
    [Theory]
    [InlineData(5, 5, true)]   // 100% — pass
    [InlineData(3, 5, true)]   // 60%  — pass (boundary)
    [InlineData(2, 5, false)]  // 40%  — fail
    [InlineData(0, 5, false)]  // 0%   — fail
    public void QuizResult_PassedShouldBeCorrect_BasedOnPercentage(int score, int total, bool expectedPassed)
    {
        // Arrange & Act
        double percentage = Math.Round((double)score / total * 100, 2);
        bool passed = percentage >= 60;

        // Assert
        Assert.Equal(expectedPassed, passed);
    }

    [Fact]
    public void QuizResult_PercentageShouldBeCalculatedCorrectly()
    {
        int score = 4, total = 5;
        double percentage = Math.Round((double)score / total * 100, 2);
        Assert.Equal(80.0, percentage);
    }
}
