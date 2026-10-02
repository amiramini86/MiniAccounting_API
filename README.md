# MiniAccounting API

A small accounting REST API built with **ASP.NET Core and .NET 10**, designed as a backend portfolio project demonstrating Clean Architecture, Domain-Driven Design (DDD), CQRS, Entity Framework Core, SQL Server, and automated testing.

The project focuses on core accounting operations and domain rules rather than implementing a complete accounting or ERP system.

## Table of Contents

* [Overview](#overview)
* [Features](#features)
* [Technology Stack](#technology-stack)
* [Architecture](#architecture)
* [Project Structure](#project-structure)
* [Domain Model](#domain-model)
* [API Endpoints](#api-endpoints)
* [Standard API Response](#standard-api-response)
* [Business Rules](#business-rules)
* [Getting Started](#getting-started)
* [Database Migrations](#database-migrations)
* [Running Tests](#running-tests)
* [Design Decisions](#design-decisions)
* [Future Improvements](#future-improvements)

## Overview

MiniAccounting is a backend application for managing accounts and journal entries.

It supports creating and retrieving accounts, recording journal entries with multiple debit and credit lines, and validating accounting rules before persisting data.

The main accounting invariant is that every journal entry must be balanced: the total debit amount must equal the total credit amount.

The project is intentionally kept small so that its architecture, business rules, persistence layer, and tests remain easy to understand and review.

## Features

* RESTful API built with ASP.NET Core.
* Clean Architecture with separate API, Application, Domain, and Infrastructure layers.
* Domain-Driven Design principles for accounting entities and business rules.
* CQRS using MediatR.
* Repository pattern and Unit of Work.
* Entity Framework Core with SQL Server.
* FluentValidation for request validation.
* Centralized exception handling middleware.
* Standardized API response wrapper.
* Swagger/OpenAPI documentation.
* Unit tests for domain rules.
* Integration tests for API endpoints and database persistence.
* Asynchronous database operations with cancellation token support.
* Optimized journal-entry queries using eager loading to retrieve related lines and accounts.

## Technology Stack

| Technology             | Purpose                   |
| ---------------------- | ------------------------- |
| C#                     | Main programming language |
| .NET 10                | Application platform      |
| ASP.NET Core Web API   | HTTP API                  |
| Entity Framework Core  | ORM and database access   |
| SQL Server             | Relational database       |
| MediatR                | CQRS request dispatching  |
| FluentValidation       | Request validation        |
| Swagger / OpenAPI      | API documentation         |
| xUnit                  | Automated testing         |
| ASP.NET Core Test Host | API integration testing   |

## Architecture

The solution follows Clean Architecture principles. Dependencies point inward toward the Domain layer.

```text
                  ┌──────────────────────┐
                  │    API / Web Layer   │
                  │ Controllers, Program │
                  └──────────┬───────────┘
                             │
                             ▼
                  ┌──────────────────────┐
                  │     Application     │
                  │ Commands, Queries,   │
                  │ Handlers, Validators │
                  │ Interfaces, DTOs     │
                  └──────────┬───────────┘
                             │
                             ▼
                  ┌──────────────────────┐
                  │        Domain        │
                  │ Entities, Value      │
                  │ Objects, Rules       │
                  └──────────────────────┘

                  ┌──────────────────────┐
                  │    Infrastructure    │
                  │ EF Core, SQL Server, │
                  │ Repositories, UoW    │
                  └──────────────────────┘
```

### API Layer

The API layer handles HTTP requests and responses.

Responsibilities include:

* Exposing REST endpoints through controllers.
* Registering application and infrastructure dependencies.
* Configuring middleware.
* Providing Swagger documentation.

### Application Layer

The Application layer coordinates use cases and contains:

* CQRS commands and queries.
* MediatR handlers.
* FluentValidation validators.
* DTOs and application exceptions.
* Repository and Unit of Work interfaces.

It does not directly depend on Entity Framework Core or SQL Server.

### Domain Layer

The Domain layer contains the core accounting model and business rules.

It includes:

* `Account`
* `JournalEntry`
* `JournalEntryLine`
* `Money`
* Domain exceptions

Business rules are enforced by the domain model rather than relying exclusively on controllers or database constraints.

### Infrastructure Layer

The Infrastructure layer implements persistence and application interfaces.

It includes:

* `MiniAccountingDbContext`
* Entity Framework Core configurations
* Repository implementations
* Unit of Work implementation
* SQL Server integration

## Project Structure

```text
MiniAccounting/
├── src/
│   ├── MiniAccounting.Api/
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   ├── MiniAccounting.Application/
│   │   ├── Accounts/
│   │   ├── JournalEntries/
│   │   └── Common/
│   │
│   ├── MiniAccounting.Domain/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   └── Exceptions/
│   │
│   └── MiniAccounting.Infrastructure/
│       ├── Persistence/
│       ├── Repositories/
│       └── UnitOfWork/
│
├── tests/
│   ├── MiniAccounting.UnitTests/
│   └── MiniAccounting.IntegrationTests/
│
├── MiniAccounting.sln
└── README.md
```

Some subfolder names may differ slightly depending on the current implementation.

## Domain Model

### Account

Represents an account that can be referenced by journal-entry lines.

Accounts contain identifying information such as a code and a name.

### JournalEntry

Represents an accounting transaction. It contains a date, description, and collection of journal-entry lines.

The entity is responsible for enforcing the journal-entry balancing rules.

### JournalEntryLine

Represents one debit or credit line within a journal entry. Each line references an existing account and contains a debit amount or a credit amount.

### Money

A value object used to represent monetary values and keep monetary concepts explicit in the domain model.

## API Endpoints

Base URL for local development:

`http://localhost:5282`

### Accounts

| Method | Endpoint             | Description               |
| ------ | -------------------- | ------------------------- |
| POST   | `/api/accounts`      | Create an account         |
| GET    | `/api/accounts/{id}` | Retrieve an account by ID |

Example request — create an account:

```json
{
  "code": "1001",
  "name": "Cash"
}
```

### Journal Entries

| Method | Endpoint                    | Description                    |
| ------ | --------------------------- | ------------------------------ |
| POST   | `/api/journal-entries`      | Create a journal entry         |
| GET    | `/api/journal-entries/{id}` | Retrieve a journal entry by ID |

Example request — create a balanced journal entry:

```json
{
  "date": "2026-10-02T10:00:00Z",
  "description": "Initial cash transaction",
  "lines": [
    {
      "accountId": 1,
      "debit": 1000.00,
      "credit": 0.00
    },
    {
      "accountId": 2,
      "debit": 0.00,
      "credit": 1000.00
    }
  ]
}
```

The account IDs must refer to accounts that already exist in the database.

The example assumes that accounts with IDs `1` and `2` have been created. Create the accounts first if your database is empty.

### Swagger

During local development, API documentation is available at:

`http://localhost:5282/swagger`

## Standard API Response

The API uses a shared response wrapper, `ApiResponse<T>`, to provide a consistent response structure.

The wrapper contains:

* `success`: indicates whether the operation succeeded.
* `data`: contains the result when applicable.
* `error`: contains error information when applicable.

The exact response depends on the endpoint and whether the request succeeds or fails.

## Business Rules

The following rules are enforced when creating a journal entry:

1. A journal entry must contain at least two lines.
2. Every referenced account must exist.
3. A line must have a valid account ID.
4. Debit and credit amounts cannot be negative.
5. A line cannot have both a positive debit and a positive credit.
6. A line cannot have both debit and credit equal to zero.
7. The journal entry must contain positive debit and credit totals.
8. Total debits must equal total credits.

For example, a journal entry with total debits of `1000` and total credits of `800` is rejected.

The API returns an appropriate client error for business-rule violations rather than treating them as unexpected server errors.

## Getting Started

### Prerequisites

Install the following:

* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
* [SQL Server](https://www.microsoft.com/sql-server)
* [Git](https://git-scm.com/)

### 1. Clone the repository

Replace the placeholder with your Git repository URL.

```bash
git clone <YOUR_REPOSITORY_URL>
cd MiniAccounting
```

### 2. Configure the database connection

Open `src/MiniAccounting.Api/appsettings.json` or the appropriate development configuration file.

Configure the `DefaultConnection` connection string to point to your SQL Server instance.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=MiniAccountingDb;User Id=sa;Password=<YOUR_PASSWORD>;TrustServerCertificate=True"
  }
}
```

Replace `<YOUR_PASSWORD>` with your local SQL Server password.

Do not commit real database credentials or other secrets to the repository. For shared or production environments, use environment variables or a secret-management solution.

### 3. Restore dependencies

From the solution root:

```bash
dotnet restore
```

### 4. Apply database migrations

```bash
dotnet ef database update \
  --project src/MiniAccounting.Infrastructure \
  --startup-project src/MiniAccounting.Api
```

If the `dotnet ef` command is unavailable, install the EF Core CLI tool:

```bash
dotnet tool install --global dotnet-ef
```

If it is already installed, update it when necessary:

```bash
dotnet tool update --global dotnet-ef
```

### 5. Run the API

```bash
dotnet run --project src/MiniAccounting.Api
```

Use the URL printed by the application. In the current development configuration, the API is expected at `http://localhost:5282`.

Open Swagger:

`http://localhost:5282/swagger`

## Database Migrations

Entity Framework Core migrations are used to keep the database schema synchronized with the application model.

To create a new migration:

```bash
dotnet ef migrations add <MigrationName> \
  --project src/MiniAccounting.Infrastructure \
  --startup-project src/MiniAccounting.Api
```

To apply pending migrations:

```bash
dotnet ef database update \
  --project src/MiniAccounting.Infrastructure \
  --startup-project src/MiniAccounting.Api
```

Use a descriptive migration name, such as `AddAccountDescription`.

## Running Tests

Run all tests from the solution root:

```bash
dotnet test
```

Build the solution:

```bash
dotnet build
```

The test projects cover domain rules and API/database integration, including successful requests and invalid journal-entry scenarios.

Integration tests use a separate test database configured by the test application factory. Make sure SQL Server is available and that the test connection string is configured correctly before running integration tests.

## Design Decisions

### Clean Architecture

Separating business rules, application use cases, HTTP concerns, and persistence makes the code easier to maintain and test.

### CQRS

Commands represent operations that change state, while queries retrieve data. MediatR dispatches these requests to their handlers.

### Repository and Unit of Work

Repositories encapsulate database access. The Unit of Work coordinates persistence changes through EF Core.

### Domain-Level Validation

Request validation checks input shape and basic constraints. Accounting invariants are also enforced by the domain model so that business rules are not dependent solely on the HTTP API.

### Centralized Exception Handling

Middleware maps known application and domain exceptions to appropriate HTTP status codes and returns a consistent error response.

### Query Optimization

Journal-entry retrieval uses eager loading to retrieve lines and their related accounts together, avoiding a separate database query for each line during response mapping.

### Database Transactions

EF Core wraps a single `SaveChangesAsync` call in a transaction when the provider supports transactions. Explicit transaction management may be needed when a use case requires multiple database saves or operations to commit atomically.

## Future Improvements

Potential next steps include:

* Docker Compose for running the API and SQL Server together.
* CI/CD pipeline for automated builds and tests.
* Authentication and authorization.
* Pagination and filtering for list endpoints.
* Additional integration tests for invalid account IDs and validation errors.
* Structured logging and production configuration.
* API versioning and health checks.

These are potential improvements and are not represented as completed features.

## License

This project is intended as a backend learning and portfolio project. Add a license file if you plan to distribute it under a specific open-source license.
