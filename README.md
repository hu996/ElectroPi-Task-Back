# Task Manager

A full-stack task management application built with ASP.NET Core Web API, Entity Framework Core, SQL Server LocalDB, and React. The system manages projects and their related tasks, with validation, duplicate prevention, status tracking, and a simple frontend for the main workflows.

## Features

- Create, read, update, and delete projects.
- Create, read, update, and delete tasks.
- View all tasks for a specific project.
- View project details with related tasks.
- Filter tasks by status.
- Update task status directly.
- Prevent duplicate project names.
- Prevent duplicate open task titles inside the same project.
- Allow the same task title again after the previous task is marked as `Done`.
- Validate request data with FluentValidation.
- Reject task due dates that are not greater than the current server date and time.
- Global exception handling middleware.
- Swagger UI for API testing.
- React frontend connected to the API.

## Tech Stack

### Backend

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core 8
- SQL Server LocalDB
- FluentValidation
- Swagger / Swashbuckle
- Repository pattern
- Service layer
- Clean Architecture style project separation

### Frontend

- React
- TypeScript
- Vite
- lucide-react icons

## Solution Structure

```text
src/
  TaskManager.Api             ASP.NET Core API, controllers, middleware, startup configuration
  TaskManager.Application     DTOs, service interfaces, services, validators, application exceptions
  TaskManager.Domain          Domain entities and enums
  TaskManager.Infrastructure  EF Core DbContext, repositories, migrations, dependency injection
  TaskManager.Client          React + TypeScript frontend
```

The backend is organized using a simplified Clean Architecture approach:

- `Domain` contains the core entities.
- `Application` contains business logic, DTOs, validators, and service contracts.
- `Infrastructure` contains persistence and repository implementations.
- `Api` exposes HTTP endpoints and configures middleware/dependency injection.
- `Client` contains the user interface.

## Domain Model

### Project

A project represents a container for tasks.

Main fields:

- `Id`
- `Name`
- `Description`
- `CreatedAt`
- `Tasks`

### TaskItem

A task belongs to one project.

Main fields:

- `Id`
- `Title`
- `Description`
- `Status`
- `DueDate`
- `ProjectId`
- `Project`

### Task Status Values

```text
ToDo
InProgress
Done
```

Enums are serialized as strings in API responses and requests.

## Database

The project uses SQL Server LocalDB by default.

Connection string location:

```text
src/TaskManager.Api/appsettings.json
```

Default connection string:

```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ElectroPiTaskManagerDb;Trusted_Connection=True;TrustServerCertificate=True"
```

EF Core migrations are included in:

```text
src/TaskManager.Infrastructure/Migrations
```

A SQL schema file is also included at the repository root:

```text
schema.sql
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server LocalDB
- Node.js and npm
- EF Core CLI tools

Install EF Core CLI if needed:

```powershell
dotnet tool install --global dotnet-ef
```

## Run Backend

From the repository root:

```powershell
dotnet restore TaskManager.sln
dotnet build TaskManager.sln
dotnet ef database update --project src\TaskManager.Infrastructure --startup-project src\TaskManager.Api --context AppDbContext
dotnet run --project src\TaskManager.Api
```

Backend URL:

```text
http://localhost:5135
```

Swagger URL:

```text
http://localhost:5135/swagger
```

HTTPS profile is also available:

```text
https://localhost:7117
```

## Run Frontend

Open another terminal:

```powershell
cd src\TaskManager.Client
npm install
npm run dev
```

Frontend URL:

```text
http://localhost:4200
```

If the API runs on a different URL, create a `.env` file inside `src/TaskManager.Client`:

```text
VITE_API_BASE_URL=http://localhost:5135
```

## API Endpoints

### Projects

| Method | Endpoint | Description |
| --- | --- | --- |
| GET | `/api/projects` | Get all projects |
| GET | `/api/projects/{id}` | Get project by id |
| GET | `/api/projects/{id}/details` | Get project with related details |
| GET | `/api/projects/{id}/tasks` | Get tasks for a project |
| POST | `/api/projects` | Create a project |
| POST | `/api/projects/{id}/tasks` | Create a task inside a project |
| PUT | `/api/projects/{id}` | Update a project |
| DELETE | `/api/projects/{id}` | Delete a project |

### Tasks

| Method | Endpoint | Description |
| --- | --- | --- |
| GET | `/api/tasks` | Get all tasks |
| GET | `/api/tasks/{id}` | Get task by id |
| GET | `/api/tasks/status/{status}` | Filter tasks by status |
| POST | `/api/tasks` | Create a task |
| PUT | `/api/tasks/{id}` | Update a task |
| PATCH | `/api/tasks/{id}/status` | Update task status |
| DELETE | `/api/tasks/{id}` | Delete a task |

## Request Examples

### Create Project

```json
{
  "name": "Website Redesign",
  "description": "Redesign the company website"
}
```

### Create Task

```json
{
  "title": "Create landing page",
  "description": "Build the first version of the landing page",
  "status": "ToDo",
  "dueDate": "2026-08-01T10:00:00",
  "projectId": 1
}
```

### Create Task Inside Project

Endpoint:

```text
POST /api/projects/1/tasks
```

Body:

```json
{
  "title": "Prepare content",
  "description": "Write the homepage content",
  "status": "InProgress",
  "dueDate": "2026-08-02T12:00:00"
}
```

### Update Task Status

```json
{
  "status": "Done"
}
```

## Validation Rules

### Project Validation

- Project name is required.
- Project name has a maximum length.
- Project description has a maximum length.
- Duplicate project names are not allowed.

### Task Validation

- Task title is required.
- Task title has a maximum length.
- Task description has a maximum length.
- Task status must be a valid enum value.
- `ProjectId` must be greater than zero when creating/updating through `/api/tasks`.
- The related project must exist before creating a task.
- `DueDate` is optional, but when provided it must be greater than the current server date and time.
- A project cannot have two open tasks with the same title when the existing task is `ToDo` or `InProgress`.

## Error Handling

The API uses a global exception middleware to return consistent error responses.

Handled cases include:

- Validation errors.
- Duplicate resources.
- Not found results.
- Unexpected server errors.

## CORS

The API allows requests from the frontend development URLs:

```text
http://localhost:4200
http://127.0.0.1:4200
```

## Build Checks

Backend:

```powershell
dotnet build TaskManager.sln
```

Frontend:

```powershell
cd src\TaskManager.Client
npm run build
```

## Design Decisions

- The backend is split into multiple projects to keep API, application logic, domain entities, and persistence separate.
- Controllers stay thin and delegate business logic to services.
- Repositories isolate EF Core data access from the service layer.
- FluentValidation is used instead of Data Annotations for cleaner validation rules.
- Swagger is enabled in development to make API testing easier.
- Nested project task endpoints, such as `/api/projects/{id}/tasks`, are included because tasks belong to projects.
- SQL Server LocalDB is used to keep local setup simple.

## Future Improvements

- Add automated unit and integration tests.
- Add authentication and authorization.
- Add pagination, sorting, and search.
- Add richer frontend notifications.
- Add Docker support for easier environment setup.