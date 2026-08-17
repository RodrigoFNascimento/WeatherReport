# WeatherReport

A study project built to explore patterns and technologies commonly used in modern .NET Web API development. This application is **not intended for production use**.

## Overview

WeatherReport is an ASP.NET Core Web API that fetches weather forecast data from the free [Open-Meteo API](https://open-meteo.com/) and returns it to consumers.
It is used as a learning sandbox to experiment with Clean Architecture, CQRS, caching, observability, resilience, scalability and testing strategies.
It's based on the Visual Studio template for web APIs - even keeping the same endpoint - but with a complete rewrite of the internal architecture and implementation.

## Architecture

The solution follows **Clean Architecture**, organized into four layers with strict inward-only dependency rules:

```
Domain  ←  Application  ←  Infrastructure  ←  WebAPI
```

| Layer | Responsibility |
|---|---|
| `Domain` | Core entities and business rules. No external dependencies. |
| `Application` | Use cases, interfaces, and MediatR handlers. Depends only on Domain. |
| `Infrastructure` | External concerns: HTTP client for Open-Meteo, Redis caching. Depends on Application. |
| `WebAPI` | Minimal API endpoints, versioning, OpenAPI docs. Depends on Infrastructure. |

Architecture rules are automatically enforced by a dedicated test project using [ArchUnitNET](https://archunitnet.readthedocs.io/).

## Features

- Weather forecast retrieval via the Open-Meteo API
- Response caching with **Redis** (distributed + output caching)
- HTTP resilience pipeline: retry, circuit breaker, and timeout policies via `Microsoft.Extensions.Http.Resilience`
- API versioning (URL segment strategy)
- OpenAPI documentation with Swagger UI
- Structured logging with **Serilog** (OpenTelemetry sink)
- Distributed tracing and metrics with **OpenTelemetry**
- Health checks for the Open-Meteo API dependency and Redis
- Docker support

## Tech Stack

| Category | Technology |
|---|---|
| Runtime | .NET 10 |
| Web framework | ASP.NET Core Minimal APIs |
| Orchestration | .NET Aspire |
| CQRS / Mediator | MediatR |
| Result handling | FluentResults + FluentResults.HttpMapping |
| Caching | Redis (Aspire.StackExchange.Redis) |
| Resilience | Microsoft.Extensions.Http.Resilience |
| Observability | OpenTelemetry, Serilog |
| API docs | Microsoft.AspNetCore.OpenApi + Swashbuckle Swagger UI |
| API versioning | Asp.Versioning |

## Project Structure

```
WeatherReport/
├── src/
│   ├── Domain/                        # Entities and domain logic
│   ├── Application/                   # Use cases, interfaces, behaviors
│   ├── Infrastructure/                # Open-Meteo client, Redis, health checks
│   ├── WebAPI/                        # Minimal API endpoints
│   ├── WeatherReport.AppHost/         # .NET Aspire host
│   └── WeatherReport.ServiceDefaults/ # Shared Aspire service defaults
└── test/
    ├── Domain.Tests.Unit/             # Domain unit tests
    ├── Application.Tests.Unit/        # Application unit tests
    ├── WebAPI.Tests.Integration/      # Integration tests (WireMock + Aspire)
    └── WeatherReport.Tests.Architecture/ # Architecture rule tests (ArchUnitNET)
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [.NET Aspire workload](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling)
- Docker (required if you don't have a Redis instance available)

### Running the application

The recommended way to run the application is via the Aspire AppHost, which automatically provisions a Redis container if no connection string is configured:

```bash
dotnet run --project src/WeatherReport.AppHost/WeatherReport.AppHost.csproj
```

The Aspire dashboard will be available at the URL printed in the console output, and provides real-time logs, traces, and metrics for all services.

To use an existing Redis instance instead of a local container, set the connection string before running:

```json
// src/WeatherReport.AppHost/appsettings.Development.json
{
  "ConnectionStrings": {
    "RedisConnection": "your-redis-connection-string"
  }
}
```

### Running the tests

Run all tests:

```bash
dotnet test
```

Run a specific test project:

```bash
# Unit tests
dotnet test test/Application.Tests.Unit/Application.Tests.Unit.csproj

# Integration tests (requires Docker for Redis)
dotnet test test/WebAPI.Tests.Integration/WebAPI.Tests.Integration.csproj

# Architecture tests
dotnet test test/WeatherReport.Tests.Architecture/WeatherReport.Tests.Architecture.csproj
```

## API Endpoints

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/v1/weather-forecast` | Returns a weather forecast |
| `GET` | `/ping` | Connectivity check |

Interactive documentation is available at `/swagger` when running in development mode.

## License

This project is released under the [MIT](LICENSE.txt) license. You are free to use, copy, modify, distribute, or do anything else you wish with this code, with no restrictions whatsoever.
