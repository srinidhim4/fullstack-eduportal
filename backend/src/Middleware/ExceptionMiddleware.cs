using System.Net;
using System.Text.Json;
using EduPortal.API.DTOs;

namespace EduPortal.API.Middleware;

/// <summary>
/// Global exception handling middleware.
/// Catches ALL unhandled exceptions and returns a consistent JSON error response.
/// Week 2 Topic: Errors and Exceptions, Middleware, Logging
/// 
/// Demonstrates:
///   - Custom middleware pattern (InvokeAsync)
///   - Exception type matching (pattern matching)
///   - Structured logging
///   - Consistent error response format (RFC 7807-inspired)
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Call next middleware in pipeline
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        // Map exception types to HTTP status codes
        // Week 2 Topic: Pattern matching (C# 8+)
        var (statusCode, message) = ex switch
        {
            ArgumentException        => (HttpStatusCode.BadRequest,   ex.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, ex.Message),
            KeyNotFoundException     => (HttpStatusCode.NotFound,     ex.Message),
            InvalidOperationException => (HttpStatusCode.Conflict,    ex.Message),
            _                        => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        context.Response.StatusCode  = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse(
            Message:    message,
            StatusCode: (int)statusCode,
            Details:    statusCode == HttpStatusCode.InternalServerError ? null : ex.Message
        );

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
