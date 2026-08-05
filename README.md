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

BookCopy inventory management is currently being implemented. The existing Stock property will eventually be replaced by copy counts calculated from individual BookCopy records.

## Planned Features

- Add and retire physical book copies
- Track available and loaned copies
- Implement checkout and return workflows
- Add library member management
- Build a React frontend

## Running Tests

```bash
dotnet test LibraryManagementSystem.sln