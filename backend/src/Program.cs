/**
 * EduPortal Backend — Program.cs
 * ASP.NET Core 8 entry point and DI configuration.
 *
 * Week 2 Topics demonstrated here:
 *   - Setting Up .NET Development Environment
 *   - Dependency Injection and Configuration
 *   - Middleware and Routing
 *   - Authentication and Authorization with JWT
 *   - Exception Handling
 *   - Using Swagger/OpenAPI
 *   - CORS
 *   - Entity Framework Core
 *   - Security
 */

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using EduPortal.API.Data;
using EduPortal.API.Middleware;
using EduPortal.API.Services;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Controllers + JSON ─────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        // Use camelCase JSON to match TypeScript frontend conventions
        opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// ── 2. Database — Entity Framework Core with SQLite ───────────────────────────
// SQLite is used here for zero-setup — swap to SQL Server for production:
//   opts.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlite(builder.Configuration.GetConnectionString("Default")
        ?? "Data Source=eduportal.db"));

// ── 3. Dependency Injection — register services ───────────────────────────────
// Week 2 Topic: Dependency Injection
// Scoped = new instance per HTTP request (correct for services using DbContext)
builder.Services.AddScoped<IAuthService,    AuthService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IJwtService,     JwtService>();

// ── 4. JWT Authentication ─────────────────────────────────────────────────────
// Week 2 Topic: JWT tokens, Security, Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? "EduPortalSuperSecretKey2024!DevOnly"; // fallback for development only

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer           = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"] ?? "EduPortal",
            ValidateAudience         = true,
            ValidAudience            = builder.Configuration["Jwt:Audience"] ?? "EduPortalUsers",
            ValidateLifetime         = true,
            ClockSkew                = TimeSpan.Zero, // No clock drift tolerance
        };
    });

builder.Services.AddAuthorization();

// ── 5. CORS — allow the frontend origin ──────────────────────────────────────
// Week 2 Topic: CORS
// In production: replace with your actual deployed frontend URL
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
        policy
            .WithOrigins(
                "http://localhost:3000",   // React dev server
                "http://localhost:5500",   // VS Code Live Server
                "http://127.0.0.1:5500",
                "http://localhost:8080",   // Common dev ports
                "null"                     // file:// origin for direct HTML open
            )
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// ── 6. Swagger / OpenAPI ──────────────────────────────────────────────────────
// Week 2 Topic: Using Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "EduPortal API",
        Version     = "v1",
        Description = "Student Course Management System — Week 2 Backend Demo",
        Contact     = new OpenApiContact { Name = "EduPortal Team" }
    });

    // Add JWT bearer auth UI in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type        = SecuritySchemeType.Http,
        Scheme      = "bearer",
        BearerFormat = "JWT",
        Description = "Paste your JWT token here (obtained from /api/auth/login)"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ── 7. Logging ────────────────────────────────────────────────────────────────
// Week 2 Topic: Exception Handling and Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ══════════════════════════════════════════════════════════════════════════════
// BUILD THE APP
// ══════════════════════════════════════════════════════════════════════════════
var app = builder.Build();

// ── Auto-create the SQLite database on startup ────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // Creates tables if they don't exist (no migrations needed for SQLite dev)
}

// ── Middleware Pipeline — ORDER MATTERS ───────────────────────────────────────
// Week 2 Topic: Middleware and Routing

// 1. Global exception handler — must be FIRST to catch all errors
app.UseMiddleware<ExceptionMiddleware>();

// 2. Swagger UI — only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EduPortal API v1");
        c.RoutePrefix = "swagger"; // Access at /swagger
    });
}

// 3. HTTPS redirection
app.UseHttpsRedirection();

// 4. CORS — must come BEFORE authentication
app.UseCors("FrontendPolicy");

// 5. Authentication (who are you?) — before Authorization
app.UseAuthentication();

// 6. Authorization (what can you do?)
app.UseAuthorization();

// 7. Map controllers (routes to controller actions)
app.MapControllers();

// 8. Health check endpoint — no auth required
// Week 2 Topic: Minimal APIs
app.MapGet("/api/health", () => new
{
    status  = "healthy",
    service = "EduPortal API",
    version = "1.0.0",
    time    = DateTime.UtcNow
}).AllowAnonymous();

app.Run();
