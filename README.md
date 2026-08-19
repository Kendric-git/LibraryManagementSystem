# Library Management System

A learning-focused library management application built with ASP.NET Core and PostgreSQL. The project is intended to demonstrate API design, separation of concerns, relational database modeling, and automated testing.

## Current Functionality

- Create, retrieve, update, and delete books
- Validate book genres
- Retrieve available genres
- Store data using PostgreSQL and Entity Framework Core
- Track individual physical books through BookCopy records
- Unit test controller behavior
- Integration test services against a PostgreSQL Testcontainer

## Technology

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- xUnit
- Moq
- Testcontainers

## Project Structure

The API separates controllers, services, DTOs, data models, and database access. Unit tests verify controller behavior, while integration tests verify service and database behavior.

## Current Development

BookCopy tests are now complete, the frontend is now being developed and other planned features will be added once the frontend is running. Further refactoring of services and tests is being considered. Lastly, the CI/CD pipeline using Github Actions will also began development.

## Planned Features

- Add and retire physical book copies
- Track available and loaned copies
- Implement checkout and return workflows
- Add library member management
- Build a React frontend

## Running Tests

```bash
dotnet test LibraryManagementSystem.sln