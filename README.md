```md

# FitnessApp Backend

ASP.NET Core backend microservices for FitnessApp. Handles authentication, exercise management, and product/calorie tracking.

## Table of Contents

- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Installation](#installation)
- [API Documentation](#api-documentation)
- [Database Setup](#database-setup)
- [Future Improvements](#future-improvements coming) 

## Architecture

The backend is split into three microservices:


AuthAPI (Port 5010) - User Authentication
ExerciseAPI (Port 5185) - Exercise Database & Logs
BackendLogicApi (Port 5142)- Product & Calorie Tracking


Each service has its own PostgreSQL database and runs independently.

### Microservice Flow


Frontend → AuthAPI → JWT validation → Access other services
Frontend → ExerciseAPI → Log exercises
Frontend → BackendLogicApi → Log products / calculate calories


## Tech Stack

- ASP.NET Core 7+
- Entity Framework Core
- PostgreSQL 12+
- JWT Authentication
- Swagger/OpenAPI
- FluentValidation

## Project Structure


FitnessApp/
├── AuthAPI/
│ ├── Controllers/
│ ├── Services/
│ ├── DataAccess/
│ └── Program.cs
├── ExerciseAPI/
│ ├── Controllers/
│ ├── Services/
│ ├── Models/
│ └── Program.cs
├── BackendLogicApi/
│ ├── Controllers/
│ ├── Services/
│ ├── Models/
│ └── Program.cs


## Installation

### Prerequisites

- .NET 7+
- PostgreSQL 12+

### Setup Services

```bash
cd FitnessApp/AuthAPI
dotnet restore
dotnet ef database update
dotnet run

Repeat for ExerciseAPI and BackendLogicApi after configuring appsettings.json for each database.

Database Setup

Create three databases for each service:

CREATE USER fitnessapp WITH PASSWORD 'your_password';
CREATE DATABASE auth OWNER fitnessapp;
CREATE DATABASE exercise OWNER fitnessapp;
CREATE DATABASE products OWNER fitnessapp;
GRANT ALL PRIVILEGES ON DATABASE auth TO fitnessapp;
GRANT ALL PRIVILEGES ON DATABASE exercise TO fitnessapp;
GRANT ALL PRIVILEGES ON DATABASE products TO fitnessapp;
API Documentation

Swagger UI is available for each service:

AuthAPI: http://localhost:5010/swagger
ExerciseAPI: http://localhost:5185/swagger
BackendLogicApi: http://localhost:5142/swagger

Endpoints cover registration, login, exercise CRUD, product logging, and calorie calculations.

Authentication
JWT token-based
Token stored in localStorage on frontend
Token expiration: 60 minutes
Secret key configured in appsettings.json
Future Improvements
Dockerize all services
Environment variable management
CI/CD pipeline
Advanced analytics & social features