# EduPortal — Student Course Management System

**Week 1 (Frontend) + Week 2 (Backend) Complete Project**

---

## Project Structure

```
EduPortal/
├── frontend/                    ← Week 1: TypeScript + HTML/CSS
│   ├── index.html
│   ├── styles.css               ← Light theme UI
│   ├── package.json
│   ├── tsconfig.json
│   └── src/
│       ├── app.ts               ← Main application logic
│       ├── course.ts            ← Course model
│       ├── quiz.ts              ← Quiz models
│       └── student.ts           ← Student model
│
└── backend/                     ← Week 2: ASP.NET Core 8
    ├── EduPortal.API.csproj     ← NuGet packages
    ├── src/
    │   ├── Program.cs           ← App entry point, DI, middleware
    │   ├── appsettings.json     ← Configuration
    │   ├── Models/
    │   │   ├── Student.cs       ← Student + Enrollment entities
    │   │   └── Course.cs        ← Course, QuizResult entities
    │   ├── Data/
    │   │   ├── AppDbContext.cs  ← EF Core DbContext
    │   │   └── CourseDataStore.cs ← Static course data + LINQ
    │   ├── DTOs/
    │   │   └── Dtos.cs          ← Records for request/response
    │   ├── Services/
    │   │   ├── JwtService.cs    ← JWT token generation
    │   │   ├── AuthService.cs   ← Register + Login logic
    │   │   └── StudentService.cs ← Enrollment, profile, quiz
    │   ├── Controllers/
    │   │   ├── AuthController.cs    ← POST /api/auth/register|login
    │   │   ├── CoursesController.cs ← GET /api/courses, POST enroll
    │   │   ├── StudentsController.cs ← GET/PUT /api/students
    │   │   └── QuizController.cs    ← POST /api/quiz/submit
    │   └── Middleware/
    │       └── ExceptionMiddleware.cs ← Global error handling
    └── tests/
        ├── EduPortal.Tests.csproj
        └── EduPortal.Tests.cs   ← xUnit + Moq unit tests
```

---

## Week 2 Topics Covered

| Topic | File(s) |
|-------|---------|
| C# Basics, Variables, Data Types | All `.cs` files |
| OOP — Classes, Interfaces, Inheritance | `Models/`, `Services/` |
| Records, Structs | `DTOs/Dtos.cs` |
| Collections, LINQ | `Data/CourseDataStore.cs`, `Services/StudentService.cs` |
| Delegates, Lambdas | `CourseDataStore.cs` (LINQ lambdas) |
| Errors and Exceptions | `Middleware/ExceptionMiddleware.cs` |
| Async Programming / Tasks | All service methods use `async Task<T>` |
| Libraries, NuGet | `EduPortal.API.csproj` |
| Extension Methods | LINQ throughout |
| Dependency Injection | `Program.cs` — `AddScoped<>` |
| File Handling / Serialization | SQLite DB via EF Core |
| OOP Polymorphism | Interface + implementation pattern |
| Generics | `ApiResponse<T>`, `Result<T>` pattern |
| .NET Framework & Core | ASP.NET Core 8 |
| Networking | HttpClient in frontend |
| Security | BCrypt passwords, JWT, HTTPS |
| Entity Framework Core | `Data/AppDbContext.cs`, `Services/` |
| Exception Handling & Logging | `Middleware/`, `ILogger<T>` throughout |
| Design Patterns | Repository pattern, DI |
| Unit Testing | `tests/EduPortal.Tests.cs` |
| Swagger/OpenAPI | `Program.cs` — Swashbuckle |
| ASP.NET Core MVC | All controllers |
| RESTful APIs | `CoursesController`, `AuthController` |
| Middleware and Routing | `Program.cs` middleware pipeline |
| Authentication & Authorization | JWT Bearer, `[Authorize]` |
| JWT Token | `Services/JwtService.cs` |
| CORS | `Program.cs` — CORS policy |
| Content Negotiation & HTTP Codes | All controllers |

---

## Quick Start

### Option A — Frontend Only (no backend needed)

The frontend works fully with localStorage when the backend is not running.

1. Open `frontend/index.html` directly in your browser **OR**
2. Use VS Code Live Server (right-click → Open with Live Server)
3. Register an account and explore all features

### Option B — Full Stack (Frontend + Backend)

#### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) (for TypeScript compilation)

#### Step 1 — Build the Frontend TypeScript

```bash
cd frontend
npm install
npm run build      # Compiles src/*.ts → dist/app.js
```

#### Step 2 — Run the Backend

```bash
cd backend

# Restore NuGet packages
dotnet restore

# Run the API (creates SQLite DB automatically)
dotnet run --project EduPortal.API.csproj
```

The API starts at: **http://localhost:5000**
Swagger UI at: **http://localhost:5000/swagger**

#### Step 3 — Open the Frontend

Open `frontend/index.html` in a browser (or use Live Server on port 5500).

The frontend auto-detects the backend — if running, it uses the API; otherwise, it falls back to localStorage.

---

## API Endpoints

```
POST   /api/auth/register          Register new student
POST   /api/auth/login             Login, returns JWT token

GET    /api/courses                List all courses
GET    /api/courses/{id}           Get one course
POST   /api/courses/{id}/enroll    Enroll in course  [JWT required]

GET    /api/students/me            Get my profile    [JWT required]
PUT    /api/students/profile       Update name       [JWT required]

POST   /api/quiz/{courseId}/submit Save quiz result  [JWT required]
GET    /api/quiz/results           Get my results    [JWT required]

GET    /api/health                 Health check (no auth)
```

---

## Run Unit Tests

```bash
cd backend
dotnet test tests/EduPortal.Tests.csproj
```

Tests cover: JWT generation, password hashing (BCrypt), input validation,
course data LINQ queries, quiz pass/fail logic.

---

## Technology Stack

### Frontend (Week 1)
- TypeScript 5 — strict mode
- Vanilla DOM manipulation
- localStorage for offline mode
- Open-Meteo API (free weather, no API key)

### Backend (Week 2)
- ASP.NET Core 8
- Entity Framework Core 8 + SQLite
- BCrypt.Net — password hashing
- JWT Bearer Authentication
- Swagger / Swashbuckle
- xUnit + Moq — unit tests
