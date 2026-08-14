# AssignFlow Backend

AssignFlow is a role-based assignment and submission management system for schools and colleges. The backend uses .NET 10, ASP.NET Core Web API, Entity Framework Core, PostgreSQL, ASP.NET Core Identity, JWT authentication, FluentValidation, Swagger/OpenAPI, and xUnit. The frontend uses Angular 21 standalone components, PrimeNG, signals, reactive forms, lazy routes, and PrimeNG Toast.

The repository contains both the backend API and the Angular 21 web client.

## Features

- Admin user management for Admin, Teacher, and Student roles.
- Class, section, academic year, subject, and course-offering management.
- Teacher-to-course assignment and student enrollment.
- Draft, publish, archive, update, and safe-delete assignment workflows.
- Student assignment discovery scoped to enrolled courses.
- Deadline-enforced submission and controlled resubmission.
- Teacher review, status changes, marks, and feedback.
- Admin visibility across assignments and submissions.
- Application settings, role-aware dashboard summaries, pagination, and filtering.
- JWT authentication, role attributes, and service-level ownership checks.
- PostgreSQL migrations, idempotent SQL script, sample data, Swagger, Docker, and tests.

## Architecture

The solution follows the CRAB API request flow and project boundaries:

```text
HTTP request
  <- AssignFlow.UI standalone Angular client
  -> AssignFlow.API controller / middleware / validation
  -> AssignFlow.Services interface and business service
  -> AssignFlow.DataAccess repository interface and repository
  -> AssignFlow.Domain AppDbContext / entity
  -> PostgreSQL
```

Entity modules expose CRAB-style `IService<TEntity, TId>` / `Service<TEntity, TId>` and
`IRepository<TEntity, TId>` / `Repository<TEntity, TId>` CRUD layers. Module-specific
interfaces extend those generic contracts only with business operations. Controllers use
classic constructor injection and return `ApiResponse<T>` through explicit typed `data` or
`result` variables, matching the existing CRAB controller convention.

| Project | Responsibility |
|---|---|
| `AssignFlow.API` | Controllers, middleware, validation, JWT/Swagger configuration, startup, and seed orchestration |
| `AssignFlow.UI` | Angular 21 standalone UI, PrimeNG design system, role-based lazy routes, guards, interceptors, types, and feature services |
| `AssignFlow.Services` | Business rules, service interfaces/implementations, and service injection |
| `AssignFlow.DataAccess` | Repository interfaces/implementations, optimized EF Core queries, and repository injection |
| `AssignFlow.Domain` | Standalone entities, enums, `AppDbContext`, relationships, indexes, and migrations |
| `AssignFlow.Models` | Standalone request/response DTOs and pagination contracts |
| `AssignFlow.Utils` | Roles and typed application exceptions |
| `AssignFlow.Tests` | Authorization and submission-workflow business-rule tests |

## Performance decisions

- List endpoints use database-side projection, filtering, ordering, pagination, and `AsNoTracking()`.
- Page size is capped at 100 to protect the API and database.
- Hot queries have composite indexes for course/status/deadline and assignment/status.
- User and role listing is a single join query; it does not execute one role query per user.
- Existence checks use `AnyAsync`; counts remain database-side.
- Repository methods accept cancellation tokens.
- No unbounded generic `ListAsync()` is used by a hot endpoint.
- Unique database constraints protect enrollment, teacher mapping, subject codes, course offerings, and one-submission-per-student rules.

## Prerequisites

- .NET SDK 10
- Node.js 22 or 24 and npm
- PostgreSQL 17 or a compatible supported PostgreSQL release
- Docker Desktop is optional but recommended

## Quick start with Docker

```bash
cp .env.example .env
docker compose up --build
```

Swagger: `http://localhost:8080/swagger`

Health endpoint: `http://localhost:8080/health`

Web application: `http://localhost:4200`

The Development container applies the migration and inserts demo data after PostgreSQL becomes healthy.

## Local setup without Docker

1. Create a PostgreSQL database and copy `AssignFlow.API/appsettings.Local.example.json` to `AssignFlow.API/appsettings.Local.json`, then enter your local database credentials. The local file is excluded from Git.
2. Alternatively, use double underscores for nested .NET configuration keys:

```powershell
$env:ConnectionStrings__DefaultConnection='Host=localhost;Port=5432;Database=assignflow;Username=assignflow;Password=your_password'
$env:Jwt__Key='replace-with-at-least-32-random-characters'
dotnet restore AssignFlow.slnx
dotnet ef database update --project AssignFlow.Domain --startup-project AssignFlow.API
dotnet run --project AssignFlow.API
```

The migration is in `AssignFlow.Domain/Migrations`. An idempotent database script is available at `database/assignflow-initial.sql`.

## Frontend setup

```powershell
cd AssignFlow.UI
npm install
ng serve
```

Open `http://localhost:4200`. The Angular client calls the IIS Express API directly at `https://localhost:44383/api`; no frontend API proxy is used.

### Run the backend with IIS Express

1. Open `AssignFlow.slnx` in Visual Studio and select `AssignFlow.API` as the startup project.
2. Select the `IIS Express` launch profile and start it. Swagger opens at `https://localhost:44383/swagger/index.html`.
3. Start Angular:

```powershell
cd AssignFlow.UI
ng serve
```

Open `http://localhost:4200`. The browser communicates directly with the IIS Express HTTPS endpoint on port `44383`.

The client uses session-scoped JWT storage, centralized API error handling, PrimeNG Toast notifications, role guards, server-side pagination, strict template checking, OnPush change detection, and lazy-loaded feature pages.

## Demo credentials

Demo seeding must be enabled with `Seed__Enabled=true`. These accounts are for local evaluation only:

| Role | Email | Password source |
|---|---|---|
| Admin | `mzhr.riad@gmail.com` | `12345678` |
| Teacher | `teacher@assignflow.local` | `AssignFlow@123` |
| Student | `student@assignflow.local` | `AssignFlow@123` |

These evaluation-only defaults are provided by `appsettings.Development.json` and `.env.example`. Change them and disable seeding outside local evaluation.

## Main API routes

| Area | Route |
|---|---|
| Authentication | `POST /api/auth/login`, `GET /api/auth/me` |
| Users/settings | `/api/admin/users`, `/api/admin/settings` |
| Classes/subjects/courses | `/api/academic/*` |
| Assignments | `/api/assignments` |
| Submissions and grading | `/api/submissions` |
| Dashboard | `GET /api/dashboard/summary` |

Authorize Swagger with `Bearer <access-token>` after calling the login endpoint.

## Tests

```bash
dotnet test AssignFlow.slnx
```

The current tests cover assigned-teacher authorization, Admin authoring restrictions, unauthorized course access, deadline enforcement, allowed resubmission, and maximum-mark validation. Controllers additionally enforce role policies, while services repeat ownership checks to prevent authorization bypass.

## Business rules

- Only a teacher assigned to a course can create or manage its assignments.
- Students can only see published assignments for courses in which they are enrolled.
- Published assignments require a future deadline.
- Submissions are rejected after the deadline.
- A second submission requires `AllowResubmission=true` and cannot replace a graded submission.
- Marks must be between zero and the assignment maximum.
- Only draft assignments without submissions can be deleted.
- Archived assignments cannot be reopened.

## Assumptions

- Angular 21 was selected at the requester's direction instead of Next.js/React; the required TypeScript, responsive UI, validation, and API integration capabilities are retained.
- A course offering represents one subject taught to one class/section in one academic year.
- Multiple teachers can share a course offering.
- Students submit a rich-text/plain-text answer; binary attachments are outside the mandatory brief.
- Late submission is disabled because the brief defines a deadline and does not explicitly allow late work.
- Admin has read-only oversight of assignments and submissions; authoring, workflow changes, and grading belong to assigned teachers.
- All persisted workflow timestamps are UTC. The future Angular client will localize them for display.

## Known limitations

- Email/push notifications and file attachments are optional and not implemented.
- Refresh tokens are not included in the recruitment brief; access tokens expire after the configured duration.
- The initial tests focus on high-risk business rules. A production rollout should add PostgreSQL-backed integration tests and load tests.

## Security notes

- Never commit `.env` or production secrets.
- Set a long random `Jwt__Key`, restrict CORS, disable demo seeding, and use a managed secret store in production.
- Automatic migration is configuration-controlled and disabled by default outside Development/Docker evaluation.
