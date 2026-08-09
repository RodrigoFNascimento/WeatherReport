# Architecture Tests

This project contains automated architecture tests using **ArchUnitNET** to ensure the WeatherReport application follows Clean Architecture principles.

## Test Categories

### 1. Clean Architecture Dependency Tests (`CleanArchitectureDependencyTests.cs`)
Validates the core dependency rules of Clean Architecture:
- **Domain** must not depend on Application, Infrastructure, or WebAPI
- **Application** must not depend on Infrastructure or WebAPI
- **Infrastructure** must not depend on WebAPI
- Domain and Application layers must be framework-agnostic (no ASP.NET, MediatR dependencies in Domain)

### 2. Naming Convention Tests (`NamingConventionTests.cs`)
Enforces consistent naming patterns across the codebase:
- Interfaces should start with "I"
- Repository interfaces should end with "Repository"
- Repository implementations should end with "Repository"
- MediatR handlers should end with "Handler"
- Settings classes should end with "Settings"
- API endpoints should end with "Endpoint"
- Health checks should end with "HealthCheck"
- MediatR behaviors should end with "Behavior"

### 3. Repository Pattern Tests (`RepositoryPatternTests.cs`)
Validates the Repository pattern implementation:
- Repository interfaces must be in the Application layer
- Repository implementations must be in the Infrastructure layer
- Application layer should not reference Infrastructure repository implementations
- WebAPI should not directly reference Infrastructure repositories
- Service interfaces should be in the Application layer
- Application should not reference Infrastructure service implementations

### 4. Layer Isolation Tests (`LayerIsolationTests.cs`)
Ensures proper separation of concerns:
- Domain and Application must not depend on Entity Framework
- Domain and Application must not have HTTP dependencies
- Domain must not use logging infrastructure
- Domain must not use configuration
- Application must not use configuration
- Only WebAPI should use ASP.NET Core MVC
- Domain must not have Dependency Injection concerns

## Running the Tests

Run all architecture tests:
```bash
dotnet test test/WeatherReport.Tests.Architecture/WeatherReport.Tests.Architecture.csproj
```

Run specific test class:
```bash
dotnet test test/WeatherReport.Tests.Architecture/WeatherReport.Tests.Architecture.csproj --filter "FullyQualifiedName~CleanArchitectureDependencyTests"
```

## Clean Architecture Principles Enforced

### Dependency Rule
The fundamental rule: source code dependencies can only point inwards. Nothing in an inner circle can know anything about something in an outer circle.

**Layers (from inner to outer):**
1. **Domain** - Entities, Value Objects, Domain logic (no external dependencies)
2. **Application** - Use cases, application logic, interfaces (depends only on Domain)
3. **Infrastructure** - External concerns, repositories, services (depends on Application)
4. **WebAPI** - Presentation, API endpoints (depends on Infrastructure)

### Key Benefits
- **Testability**: Inner layers can be tested without outer layer dependencies
- **Maintainability**: Changes in outer layers don't affect inner layers
- **Flexibility**: Easy to swap implementations (e.g., different databases, frameworks)
- **Framework Independence**: Core business logic independent of frameworks

## Technologies Used
- **ArchUnitNET** - Architecture testing library for .NET
- **xUnit** - Test framework
- **.NET** - Target framework

## Continuous Integration
These tests should be run as part of your CI/CD pipeline to catch architecture violations early in the development process.

## Adding New Tests
When adding new architecture rules:
1. Create a new test method with `[Fact]` attribute
2. Use the ArchUnitNET fluent API to define the rule
3. Include a clear `.Because()` message explaining the rule
4. Run tests to verify the rule works as expected
5. Update this README if adding a new test category
