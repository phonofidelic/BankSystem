# BankSystem

A backend banking system built with C# and ASP.NET Core as part of the .NET Fullstack System Developer program at Lexicon.

The project focuses on backend architecture, domain modeling, authentication, persistence, testing, and cloud deployment. It is structured as separate Domain, Application, Infrastructure, and API projects to keep business logic and infrastructure concerns clearly separated.

## Tech Stack

* C#
* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* JWT authentication
* xUnit
* Docker
* Azure Container Apps
* Azure SQL
* Azure Container Registry
* Azure Key Vault

## Architecture

The solution is split into several projects:

* **BankSystem.Domain** — core domain entities and business concepts
* **BankSystem.Application** — application services, use cases, interfaces, commands, and queries
* **BankSystem.Infrastructure** — persistence, repositories, identity, authentication, and other infrastructure implementations
* **BankSystem.Api** — REST API, dependency injection, authentication, authorization, and application configuration
* **BankSystem.Application.Tests** — tests for application-level behavior and use cases
* **BankSystem.Api.Tests** — API and integration tests

The application uses repository and unit-of-work abstractions to separate application logic from persistence concerns.

## Features

The API supports banking workflows including:

* Customer account management
* Bank account access
* Deposits and withdrawals
* Transaction history
* Paginated account and transaction queries
* Authentication with JWT tokens
* Role-based authorization
* System administration and customer-service roles
* Audit logging
* Currency-related services

Application behavior is implemented through explicit command/query handlers and use cases, including:

* Open customer account
* Update customer account
* Close customer account
* Get customer account details
* List customer accounts
* Make deposits
* Make withdrawals
* List transactions for an account
* List all transactions

## Authentication and Authorization

Authentication is implemented using ASP.NET Core Identity and JWT bearer tokens.

The API includes role-based authorization policies for:

* Authenticated users
* Customer service representatives
* System administrators

JWT issuer, audience, signing secret, and administrator credentials are configured externally rather than being hard-coded in the application.

## Persistence

The application uses Entity Framework Core with SQL Server.

Database migrations are applied automatically in the development environment, together with development seed data for identity and currency information.

## Testing

The solution contains separate test projects for both application behavior and API-level testing.

Testing tools include:

* xUnit
* ASP.NET Core `WebApplicationFactory`
* EF Core InMemory provider
* SQLite for integration testing
* Coverlet for test coverage

## Docker

The API includes a Dockerfile and can be built and run as a container.

## Azure Deployment

The repository includes scripts for provisioning and deploying the application to Azure.

The provisioning script creates and configures:

* Azure Resource Group
* Azure Container Registry
* Azure SQL Server and Database
* Azure Key Vault
* User-assigned Managed Identity
* Azure Container Apps environment
* Azure Container App

Application secrets such as the database connection string, JWT secret, and administrator credentials are stored in Azure Key Vault.

Managed Identity and RBAC are used to allow the deployed application to access Key Vault and pull images from Azure Container Registry without embedding cloud credentials in the application.

## API Documentation

In development, the API exposes OpenAPI documentation with Scalar.

## Running Locally

### Requirements

* .NET 10 SDK
* SQL Server
* Docker, if running the containerized version

Clone the repository:

```bash
git clone https://github.com/phonofidelic/BankSystem.git
cd BankSystem
```

Configure the required development settings, including the SQL Server connection string and JWT configuration.

Then run:

```bash
dotnet restore
dotnet run --project BankSystem.Api
```

The application will apply pending database migrations when running in the development environment.

## Running Tests

```bash
dotnet test
```

## Project Context

This project was developed during the Lexicon .NET Fullstack System Developer program as a practical exercise in building a larger backend system with layered architecture, persistence, authentication, automated testing, containerization, and cloud deployment.
