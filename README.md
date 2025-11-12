TaskManagerAPI

A simple .NET 8 Web API for task management.
Supports creating, reading, updating, and deleting tasks with an InMemory database, DTOs, and Swagger UI for testing endpoints.

🧩 Tech Stack

ASP.NET Core 8

Entity Framework Core (InMemory)

Swagger / OpenAPI

🚀 Run
dotnet restore
dotnet run
# Open Swagger at: http://localhost:5000/swagger

📂 Project Structure

Entities/ → Data models

DTOs/ → Data Transfer Objects

Data/ → EF Core context

Controllers/ → API endpoints
The InMemory database is not persistent — data resets on each run.

Replace with SQL Server or PostgreSQL for production.
