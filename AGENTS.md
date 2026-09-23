# AI Agent Guidelines for Core API Clean Additional Service

This document provides specific guidelines for AI coding agents (including Mavin AI) working on this .NET API project to ensure consistent, high-quality code generation without architectural drift.

## Project Overview

This is a Clean Architecture .NET 10.0 Web API project implementing basic CRUD operations for Book and Person entities. The project uses:
- .NET 10.0
- Entity Framework Core with SQLite
- Clean Architecture patterns (Director pattern, Repository pattern, Unit of Work)
- Serilog for logging
- Swagger/OpenAPI for API documentation

## Project Structure

```
CoreAPICleanAdditionalService/          # Main API project
├── Controllers/                        # API Controllers
├── Code/                              # Application configuration
└── Program.cs                         # Application entry point

CoreLibraryCleanAdditionalService/     # Core library project
├── Models/                            # Domain models and DTOs
├── Repository/                        # Data access layer
│   ├── Data/                         # Repository implementations
│   ├── EF/                           # Entity Framework specific
│   └── UnitOfWork/                   # Unit of Work pattern
├── Director/                          # Business logic orchestration
├── Mapper/                            # Object mapping
├── Messaging/                         # Message publishing interfaces
└── Log/                              # Logging interfaces
```

## Architecture Patterns

### Clean Architecture Principles
- **Separation of Concerns**: API layer (Controllers) → Director layer (Business logic) → Repository layer (Data access)
- **Dependency Inversion**: Depend on abstractions (interfaces) not concrete implementations
- **Single Responsibility**: Each class has one clear purpose

### Key Patterns Used
1. **Director Pattern**: Business logic coordinators in `CoreLibraryCleanAdditionalService/Director/`
2. **Repository Pattern**: Data access abstractions in `CoreLibraryCleanAdditionalService/Repository/`
3. **Unit of Work Pattern**: Transaction management in `CoreLibraryCleanAdditionalService/Repository/UnitOfWork/`
4. **DTO Pattern**: Data Transfer Objects in `CoreLibraryCleanAdditionalService/Models/`

## Coding Standards

### C# Conventions
- Use `PascalCase` for classes, methods, and public properties
- Use `camelCase` for method parameters and local variables
- Use `_camelCase` for private fields
- Use `IPascalCase` for interfaces
- Enable `ImplicitUsings` but avoid excessive using statements
- Keep `Nullable` disabled (current project setting)

### File Organization
- One class per file
- File name matches class name
- Group related functionality in appropriate folders
- Follow existing folder structure

### Dependency Injection
- Register services in `CoreAPICleanAdditionalService/Code/DependencyInjection.cs`
- Use appropriate service lifetimes:
  - `AddSingleton` for stateless services
  - `AddScoped` for per-request services (repositories, DbContext)
  - `AddTransient` for lightweight services (directors)

### Entity Framework
- Use DbContext from `SqlDataBaseDataContext`
- Enable detailed errors and sensitive data logging in development
- Use migrations for schema changes
- Ensure database is created on startup via `CreateDbIfNotExists`

### API Controllers
- Follow RESTful conventions
- Return appropriate HTTP status codes
- Use DTOs for request/response, not domain entities
- Implement proper error handling
- Add XML documentation comments for public APIs
- Use standardized `ApiResponse<T>` wrapper for all responses
- Include correlation IDs in all responses
- Return proper error responses using `ApiErrorResponse`

### Response Model Standards
- Always use `ApiResponse<T>` wrapper for successful responses
- Use `ApiErrorResponse` for error responses
- Include correlation ID in all responses
- Add pagination metadata for list responses
- Return consistent error codes and messages
- Include timestamp and request ID in all responses
- Follow the established response structure from Phase 0

### Resilience Patterns
- Use circuit breaker pattern for external service calls
- Implement retry logic with exponential backoff
- Add fallback mechanisms for service failures
- Configure appropriate circuit breaker thresholds
- Monitor circuit breaker states and transitions
- Never let external service failures break the entire API

### Rate Limiting Implementation
- Use distributed rate limiting with Redis
- Configure appropriate rate limits per user role
- Add rate limit headers to all responses
- Return 429 status code when limits are exceeded
- Implement rate limiting at the middleware level
- Log rate limiting violations for monitoring

## Configuration Management

### Application Settings
- Configuration in `appsettings.json` and `appsettings.Development.json`
- Use `IConfiguration` for accessing settings
- Environment-specific configuration via `ASPNETCORE_ENVIRONMENT`

### Connection Strings
- Store in `appsettings.json` under `ConnectionStrings:SqliteDBContext`
- Support for environment-specific connection strings

## Testing Guidelines

### Unit Tests
- Test business logic in Directors
- Mock repository dependencies
- Test edge cases and error conditions
- Arrange-Act-Assert pattern

### Integration Tests
- Test API endpoints
- Use in-memory database or test SQLite database
- Test full request/response cycle

## Build and Run Commands

```bash
# Build the solution
dotnet build

# Run the application
dotnet run

# Run in specific environment
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Add Entity Framework migrations
dotnet ef migrations add MigrationName --project CoreLibraryCleanAdditionalService

# Update database
dotnet ef database update --project CoreLibraryCleanAdditionalService
```

## Verification Steps

After making changes, verify:
1. Solution builds without errors: `dotnet build`
2. No compilation warnings
3. Follow existing code patterns and style
4. Update/add appropriate tests
5. Update documentation if API changes are made
6. Ensure database migrations are created if schema changes
7. Test API response format if response model changes are made
8. Verify circuit breaker configurations if resilience patterns are added
9. Test rate limiting rules if rate limiting is implemented

## Planned Enhancements

The following features are planned for iterative development:
1. **Standardized API Response Model**: Implement consistent API response format with correlation tracking
2. **Redis Distributed Cache**: Add caching layer for frequently accessed data
3. **RabbitMQ Message Broker**: Implement message publishing for event-driven architecture
4. **Auth0 Authentication/Authorization**: Add OAuth2/OIDC authentication and role-based authorization
5. **Circuit Breaker Pattern**: Implement resilience patterns for external service calls
6. **Rate Limiting**: Implement API rate limiting for abuse prevention and resource protection

When implementing these features:
- Follow Clean Architecture principles
- Add appropriate interfaces in CoreLibrary
- Implement in separate layers (infrastructure concerns separate from domain)
- Update dependency injection configuration
- Add configuration settings
- Provide migration guides if breaking changes
- Follow the established implementation order (Phase 0 → Phase 5)

## Common Pitfalls to Avoid

1. **Direct Database Access in Controllers**: Always use Directors → Repositories
2. **Business Logic in Controllers**: Keep controllers thin, move logic to Directors
3. **Missing Abstractions**: Always depend on interfaces, not concrete classes
4. **Ignoring Transactions**: Use Unit of Work for multi-operation transactions
5. **Hard-coded Configuration**: Use configuration files and dependency injection
6. **Breaking Clean Architecture**: Keep dependencies flowing inward (API → Library)
7. **Inconsistent API Responses**: Always use standardized response wrappers
8. **Ignoring Correlation IDs**: Always include correlation tracking for debugging
9. **Missing Circuit Breakers**: External service calls should have circuit breakers
10. **No Rate Limiting**: Implement rate limiting to prevent abuse
11. **Breaking Changes Without Versioning**: Use API versioning for breaking changes

## When to Ask for Clarification

- Requirements are ambiguous or conflicting
- Need to choose between multiple valid architectural approaches
- Changes might break existing functionality
- Performance implications are unclear
- Security considerations are involved
- Database schema changes are required

## Security Considerations

- Never commit secrets or API keys
- Use environment variables or secure configuration for sensitive data
- Implement proper input validation
- Use parameterized queries (EF Core handles this)
- Follow OWASP guidelines for API security
- Implement proper authentication/authorization when adding Auth0

## Performance Considerations

- Use async/await for I/O operations
- Consider caching for frequently accessed data (Redis integration planned)
- Optimize EF Core queries (avoid N+1 problems)
- Use pagination for large result sets
- Monitor database query performance
