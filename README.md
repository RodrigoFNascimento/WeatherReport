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

### Staging and production telemetry

Local development should continue to use the Aspire AppHost.
For staging and production, `docker-compose.yml` runs the Web API with an OpenTelemetry Collector sidecar instead.
The API sends OTLP telemetry to the collector, and the collector forwards traces, metrics, and logs to Datadog through the generic OTLP/HTTP exporter.

Set the Datadog API key before starting the stack. Configure the remote Redis connection string in the settings file for the selected environment:

```bash
$env:DD_API_KEY = "your-datadog-api-key"
docker compose up --build
```

Use `src/WebAPI/appsettings.Staging.json` for staging and `src/WebAPI/appsettings.Production.json` for production.
Replace the empty `ConnectionStrings:RedisConnection` value in the appropriate file before deployment.
These files also configure the Serilog OTLP endpoint to use the collector at `http://otel-collector:4317`.
`ASPNETCORE_ENVIRONMENT` defaults to `Staging` and can be set to `Production` for production deployments.

The default Datadog OTLP endpoint is `https://otlp.datadoghq.com`.
Override `DD_OTLP_ENDPOINT` when using another Datadog site or an intermediate endpoint.

The collector configuration is in `deploy/otel-collector.yaml`.
It accepts OTLP over gRPC on port `4317` and HTTP on port `4318`,
then exports all three signal types using `otlphttp/datadog`.
The Datadog API key is read from `DD_API_KEY` at runtime and is not stored in the repository.
The collector's health endpoint is available only inside the Compose network on port `13133`.

To use an existing Redis instance with Aspire locally, set the connection string in:

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

Run mutation tests:

```bash
# Navigate to the target unit test project directory and execute:
dotnet stryker
```

## API Endpoints

| Method | Route | Description |
|---|---|---|
| `GET` | `/alive` | Liveness check endpoint |
| `GET` | `/health` | Health check endpoint |
| `GET` | `/ping` | Connectivity check |
| `GET` | `/api/v1/weather-forecast` | Returns a weather forecast |

Interactive documentation is available at `/swagger` when running in development mode.

## License

This project is released under the [MIT](LICENSE.txt) license.
