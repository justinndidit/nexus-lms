# University LMS API

A modular ASP.NET Core Web API for building a university Learning Management System (LMS), with current focus on authentication, user identity, and role-based access foundations.

## Project Status

This project is in active development.

- Implemented now:
	- User registration flow (`POST /api/v1/auth/register`)
	- Password hashing with BCrypt
	- JWT + refresh-token utilities
	- EF Core SQL Server persistence and migrations
	- Automatic role seeding on startup
- In progress / scaffolded:
	- Login, refresh, logout, and password reset endpoints are present but currently placeholder/stub responses at controller level
	- Additional domain modules (`Academic`, `Assessment`, `Enrollment`, `Notification`) are scaffolded for upcoming work

## Vision (University Context)

The target platform supports university operations across:

- Identity and access for students, lecturers, exam officers, deans, and administrators
- Academic session/course structure
- Enrollment and registration workflows
- Assessments, grading, and role-sensitive approvals
- Notifications and student communication

Current implementation focuses on secure identity as the first foundational layer.

## Tech Stack

- .NET 10 (`net10.0`)
- ASP.NET Core Web API
- Entity Framework Core (`Microsoft.EntityFrameworkCore.SqlServer`)
- SQL Server
- JWT (`Microsoft.AspNetCore.Authentication.JwtBearer`, `System.IdentityModel.Tokens.Jwt`)
- BCrypt (`BCrypt.Net-Next`)
- DotNetEnv (`.env` loading)

## Solution Structure

```text
LmsApi.sln
src/
	webApi/
		Program.cs
		Data/
			LMSApiApplicationContext.cs
			Migrations/
			Seeders/
		Modules/
			Auth/
				Api/Controllers/
				Application/
				Domain/
				Infrastructure/
			Users/
			Academic/
			Assessment/
			Enrollment/
			Notification/
```

Architecture follows a modular, layered style:

- `Api`: controllers and HTTP entry points
- `Application`: use cases, DTOs, mappers, service orchestration
- `Domain`: entities and contracts/interfaces
- `Infrastructure`: repositories and external integrations

## Core Domain and Seeded Roles

### Persisted entities

- Users
- Roles
- Permissions
- UserRoles
- RolePermissions
- RefreshTokens
- VerificationTokens

### Seeded university roles (on startup)

- `system_admin`
- `registrar_officer`
- `exam_officer`
- `dean_of_faculty`
- `sub_dean`
- `head_of_department`
- `department_exam_officer`
- `professor`
- `lecturer`
- `lecturer_in_charge`
- `guest_lecturer`
- `teaching_assistant`
- `student`
- `course_rep`
- `post_graduate`

Default role target during user creation is `student`.

## Configuration

The app reads config from `appsettings.json` and environment variables. `DotNetEnv` is enabled, so a `.env` file can be used.

### Required settings

- `ConnectionStrings__DefaultSqlConnectionString`
- `JwtConfig__JwtSecret`
- `JwtConfig__JwtExpirationInMinutes` (optional override, default in `appsettings.json`)
- `JwtConfig__RefreshTokenExpirationInMinutes` (optional override, default in `appsettings.json`)

### Example `.env`

```env
ConnectionStrings__DefaultSqlConnectionString=Server=localhost,1433;Database=LmsApiDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;
JwtConfig__JwtSecret=replace-with-a-long-random-secret-key
JwtConfig__JwtExpirationInMinutes=15
JwtConfig__RefreshTokenExpirationInMinutes=10080
```

## Getting Started

## 1) Prerequisites

- .NET 10 SDK
- SQL Server instance (local, containerized, or remote)
- `dotnet-ef` tool (for migration commands)

Install EF tool if needed:

```bash
dotnet tool install --global dotnet-ef
```

## 2) Restore and Build

```bash
dotnet restore ./src/webApi/webApi.csproj
dotnet build ./src/webApi/webApi.csproj
```

## 3) Run the API

```bash
dotnet run --project ./src/webApi
```

Default development URL (from launch settings):

- `http://localhost:5181`

Health check root route:

- `GET /` returns `hello world`

## Database and Migrations

On application startup:

- Pending EF Core migrations are applied automatically
- Role seeding runs automatically

Manual migration flow:

```bash
dotnet ef migrations add <MigrationName> --project ./src/webApi -o Data/Migrations
dotnet ef database update --project ./src/webApi
```

## Makefile Commands

If you use `make`, the project includes shortcuts:

- `make run`
- `make watch`
- `make build`
- `make restore`
- `make migration-add name=YourMigration`
- `make migration-apply`
- `make docker-up`
- `make docker-down`

Note: `Dockerfile` and `docker-compose.yml` are currently empty scaffolds.

## API (Current)

Base path: `/api/v1/auth`

### 1) Register User

- Method: `POST`
- Route: `/api/v1/auth/register`
- Body:

```json
{
	"email": "student@university.edu",
	"password": "Secret123"
}
```

Expected behavior:

- Creates a user
- Hashes password with BCrypt
- Attempts default role assignment (`student`)
- Returns created response envelope

### 2) Other auth endpoints (stubbed at controller)

- `POST /api/v1/auth/login`
- `POST /api/v1/auth/refresh`
- `POST /api/v1/auth/logout`
- `POST /api/v1/auth/password/forget`
- `POST /api/v1/auth/password/reset`

These routes currently return placeholder strings and should be wired to service methods in upcoming iterations.

## Short Roadmap

## Auth and Identity

- Wire login/refresh/logout controller actions to `IAuthService`
- Add email verification and password reset token lifecycle completion
- Add authorization policies/role guards

## Academic Module

- Faculties, departments, sessions/semesters
- Courses and instructor assignments

## Enrollment Module

- Student course registration and approval workflow
- Add/drop period constraints

## Assessment Module

- Continuous assessment and exam components
- Grade computation, publish/review workflow

## Notification Module

- Event-driven notifications (email/in-app)
- Audit trail for critical academic operations

## Notes

- HTTPS redirection is currently commented out in startup; enable it when deploying behind proper certificates/proxy.
- Keep secrets out of source control. Prefer environment variables or secret stores in production.
