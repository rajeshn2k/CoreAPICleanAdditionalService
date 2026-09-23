# Development Guidelines - Core API Clean Additional Service

This document provides comprehensive coding standards, development practices, and guidelines for contributing to this .NET API project.

## Table of Contents
- [Code Style and Conventions](#code-style-and-conventions)
- [Project Structure Guidelines](#project-structure-guidelines)
- [Entity Framework Guidelines](#entity-framework-guidelines)
- [API Development Guidelines](#api-development-guidelines)
- [Resilience Patterns Guidelines](#resilience-patterns-guidelines)
- [Rate Limiting Guidelines](#rate-limiting-guidelines)
- [Testing Guidelines](#testing-guidelines)
- [Git Workflow](#git-workflow)
- [Documentation Standards](#documentation-standards)
- [Performance Guidelines](#performance-guidelines)
- [Security Guidelines](#security-guidelines)
- [Debugging and Troubleshooting](#debugging-and-troubleshooting)

---

## Code Style and Conventions

### C# Coding Standards

#### Naming Conventions
- **Classes**: PascalCase - `public class BookService`
- **Interfaces**: PascalCase with 'I' prefix - `public interface IBookService`
- **Methods**: PascalCase - `public async Task<Book> GetBookAsync()`
- **Properties**: PascalCase - `public string BookName { get; set; }`
- **Local Variables**: camelCase - `var bookId = "123"`
- **Private Fields**: _camelCase - `private readonly IBookRepository _bookRepository`
- **Constants**: PascalCase - `public const int MaxRetries = 3`
- **Parameters**: camelCase - `public void ProcessBook(string bookId)`

#### File Organization
- One class per file
- File name matches class name exactly
- Use folders to organize related functionality
- Keep files under 500 lines when possible
- Use `#region` for organizing large files (use sparingly)

#### Code Formatting
- Use 4 spaces for indentation (no tabs)
- Opening braces on new line for methods, classes, namespaces
- Opening braces on same line for control structures
- Max line length: 120 characters
- One blank line between methods
- No trailing whitespace

#### Example Code Structure
```csharp
namespace Core.Library.Clean.AdditionalService
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BookService> _logger;

        public BookService(IBookRepository bookRepository, ILogger<BookService> logger)
        {
            _bookRepository = bookRepository;
            _logger = logger;
        }

        public async Task<Book> GetBookAsync(string bookId, CancellationToken cancellationToken = default)
        {
            try
            {
                var book = await _bookRepository.GetByIdAsync(bookId, cancellationToken);
                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving book with ID: {BookId}", bookId);
                throw;
            }
        }
    }
}
```

### Async/Await Guidelines
- Use `async`/`await` for all I/O operations
- Always pass `CancellationToken` to async methods
- Use `ConfigureAwait(false)` in library code
- Avoid `async void` (except for event handlers)
- Use `Task.WhenAll` for parallel independent operations

#### Good Example
```csharp
public async Task<IEnumerable<Book>> GetBooksAsync(CancellationToken cancellationToken = default)
{
    var books = await _bookRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
    return books;
}
```

#### Bad Example
```csharp
public IEnumerable<Book> GetBooks()
{
    var books = _bookRepository.GetAllAsync().Result; // Avoid .Result
    return books;
}
```

### Exception Handling Guidelines
- Use specific exception types
- Log exceptions with context
- Don't catch generic `Exception` unless necessary
- Use custom exceptions for domain-specific errors
- Provide meaningful error messages

#### Good Example
```csharp
try
{
    var book = await _bookRepository.GetByIdAsync(bookId, cancellationToken);
    if (book == null)
    {
        throw new BookNotFoundException($"Book with ID {bookId} not found");
    }
    return book;
}
catch (SqlException ex)
{
    _logger.LogError(ex, "Database error while retrieving book {BookId}", bookId);
    throw new DataAccessException("Failed to retrieve book from database", ex);
}
```

### LINQ Guidelines
- Use method syntax for complex queries
- Use query syntax for simple readable queries
- Avoid deferred execution when not needed
- Use `FirstOrDefault()` instead of `First()` when not sure of existence
- Use `Any()` instead of `Count() > 0` for existence checks

---

## Project Structure Guidelines

### Solution Structure
```
Core.API.Clean.AdditionalService.sln
├── CoreAPICleanAdditionalService/          # API Project
│   ├── Controllers/                        # API Controllers
│   ├── Code/                              # Configuration
│   ├── Properties/                        # Launch settings
│   └── wwwroot/                           # Static files
└── CoreLibraryCleanAdditionalService/      # Core Library
    ├── Models/                            # Domain models
    ├── Director/                          # Business logic
    ├── Repository/                        # Data access
    ├── Mapper/                            # Object mapping
    ├── Messaging/                         # Messaging contracts
    └── Log/                              # Logging interfaces
```

### When to Add New Projects
- Add new projects only when necessary for separation of concerns
- Consider adding:
  - Test project: `CoreAPICleanAdditionalService.Tests`
  - Integration test project: `CoreAPICleanAdditionalService.IntegrationTests`
  - Message consumer project: `AdditionalService.MessageConsumer`
- Avoid project explosion - use folders first

### Folder Organization
- Group related classes in appropriate folders
- Use namespace hierarchy matching folder structure
- Keep folder depth ≤ 4 levels
- Use descriptive folder names

---

## Entity Framework Guidelines

### DbContext Usage
- Use `DbContext` with scoped lifetime
- Enable detailed errors and sensitive data logging in development only
- Use connection strings from configuration
- Dispose of DbContext properly (dependency injection handles this)

#### Configuration Example
```csharp
services.AddDbContext<SqlDataBaseDataContext>(options =>
{
    options.UseSqlite(configuration.GetConnectionString("SqliteDBContext"));
    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
        options.LogTo(Console.WriteLine, LogLevel.Information);
    }
});
```

### Query Guidelines
- Use `async` methods for database operations
- Avoid N+1 query problems
- Use `Include()` for eager loading when needed
- Use `AsNoTracking()` for read-only queries
- Parameterize queries (EF Core handles this automatically)

#### Good Example
```csharp
public async Task<Book> GetBookWithPersonAsync(string bookId, CancellationToken cancellationToken = default)
{
    var book = await _dbContext.Books
        .Include(b => b.Person)
        .AsNoTracking()
        .FirstOrDefaultAsync(b => b.Id == bookId, cancellationToken);
    
    return book;
}
```

### Migration Guidelines
- Always use migrations for schema changes
- Create descriptive migration names
- Review migration files before applying
- Test migrations on development database first
- Rollback migrations if issues occur

#### Migration Commands
```bash
# Create migration
dotnet ef migrations Add AddBookIndex --project CoreLibraryCleanAdditionalService

# Apply migration
dotnet ef database update --project CoreLibraryCleanAdditionalService

# Rollback migration
dotnet ef database update PreviousMigration --project CoreLibraryCleanAdditionalService
```

### Database Design Guidelines
- Use appropriate data types
- Add indexes for frequently queried columns
- Use foreign key constraints for relationships
- Set appropriate column lengths
- Use computed columns for derived data

---

## API Development Guidelines

### API Response Model Guidelines

#### Standardized Response Structure
- Always use `ApiResponse<T>` wrapper for successful responses
- Use `ApiErrorResponse` for error responses
- Include correlation ID in all responses
- Add timestamp and request ID to all responses
- Include pagination metadata for list responses

#### Response Wrapper Implementation
```csharp
// Success response
public async Task<ActionResult<ApiResponse<BookDTO>>> GetBook(string id)
{
    var book = await _bookDirector.GetEntityByIdAsync(id, default);
    var response = new ApiResponse<BookDTO>
    {
        Success = true,
        Data = book,
        Message = "Book retrieved successfully",
        Timestamp = DateTime.UtcNow,
        RequestId = HttpContext.Items["CorrelationId"]?.ToString()
    };
    return Ok(response);
}

// Error response
public async Task<ActionResult<ApiResponse<BookDTO>>> GetBook(string id)
{
    try
    {
        var book = await _bookDirector.GetEntityByIdAsync(id, default);
        if (book == null)
        {
            var errorResponse = new ApiErrorResponse
            {
                Success = false,
                Error = new ErrorDetail
                {
                    Code = ErrorCodes.NOT_FOUND,
                    Message = "Book not found",
                    StatusCode = 404
                },
                Timestamp = DateTime.UtcNow,
                RequestId = HttpContext.Items["CorrelationId"]?.ToString(),
                Path = $"/api/Book/{id}"
            };
            return NotFound(errorResponse);
        }
        // ... success case
    }
    catch (Exception ex)
    {
        // ... error handling
    }
}
```

#### Correlation ID Guidelines
- Always include correlation ID in log entries
- Pass correlation ID through the request pipeline
- Include correlation ID in all external service calls
- Use correlation ID for debugging and tracing

#### Pagination Guidelines
- Use `PaginationMetadata` for list responses
- Include pagination information in response headers
- Support pagination parameters (page, pageSize)
- Validate pagination parameters
- Return reasonable default page sizes

### Controller Guidelines
- Keep controllers thin - delegate to Directors
- Use appropriate HTTP verbs and status codes
- Return DTOs, not domain entities
- Add XML documentation comments
- Use async actions for I/O operations

#### Controller Example
```csharp
/// <summary>
/// Controller for managing books
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly BookDirector _bookDirector;

    public BookController(BookDirector bookDirector)
    {
        _bookDirector = bookDirector;
    }

    /// <summary>
    /// Gets all books
    /// </summary>
    /// <returns>List of books</returns>
    [HttpGet]
    public async Task<IEnumerable<BookDTO>> Get()
    {
        var books = await _bookDirector.GetEntitiesAsync(default);
        return books;
    }
}
```

### HTTP Status Code Guidelines
- `200 OK`: Successful GET, PUT, PATCH
- `201 Created`: Successful POST
- `204 No Content`: Successful DELETE
- `400 Bad Request`: Invalid input
- `401 Unauthorized`: Authentication required
- `403 Forbidden`: Authorization failed
- `404 Not Found`: Resource not found
- `409 Conflict`: Resource conflict
- `500 Internal Server Error`: Server error

### DTO Guidelines
- Use DTOs for all API communication
- Separate DTOs for create, update, and response
- Validate DTOs using data annotations
- Use appropriate naming for DTO properties
- Keep DTOs simple and focused

#### DTO Example
```csharp
public class BookDTO
{
    public string Id { get; set; }
    public string BookName { get; set; }
    public string BookCategory { get; set; }
    public double Price { get; set; }
}

public class BookCreateDTO
{
    [Required]
    [StringLength(255)]
    public string BookName { get; set; }
    
    [Required]
    public string BookCategory { get; set; }
    
    [Range(0, double.MaxValue)]
    public double Price { get; set; }
}
```

### Validation Guidelines
- Use data annotations for DTO validation
- Implement custom validators for complex validation
- Return validation errors with proper structure
- Validate at the earliest appropriate layer
- Use fluent validation if needed

---

## Resilience Patterns Guidelines

### Circuit Breaker Implementation
- Use circuit breaker pattern for all external service calls
- Implement appropriate circuit breaker thresholds per service
- Add retry logic with exponential backoff
- Implement fallback mechanisms for service failures
- Monitor circuit breaker states and transitions
- Log circuit breaker state changes for debugging

#### Circuit Breaker Configuration
```csharp
// Configure circuit breaker for Redis
var redisCircuitBreaker = Policy
    .Handle<RedisException>()
    .CircuitBreakerAsync(
        exceptionsAllowedBeforeBreaking: 5,
        durationOfBreak: TimeSpan.FromSeconds(30),
        onBreak: (exception, duration) => 
        {
            _logger.LogWarning(exception, "Redis circuit breaker opened for {Duration}", duration);
        },
        onReset: () => 
        {
            _logger.LogInformation("Redis circuit breaker reset");
        },
        onHalfOpen: () => 
        {
            _logger.LogInformation("Redis circuit breaker half-open");
        });
```

#### Retry Policy Implementation
```csharp
// Implement retry with exponential backoff
var retryPolicy = Policy
    .Handle<Exception>()
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        onRetry: (exception, timeSpan, retryCount, context) =>
        {
            _logger.LogWarning(exception, "Retry {RetryCount} after {Delay}s", retryCount, timeSpan.TotalSeconds);
        });
```

#### Fallback Strategy
```csharp
// Implement fallback for service failures
var fallbackPolicy = Policy<HttpResponseMessage>
    .Handle<Exception>()
    .FallbackAsync(
        fallbackValue: CreateFallbackResponse(),
        onFallbackAsync: async (exception, context) =>
        {
            _logger.LogError(exception, "Service call failed, using fallback");
            await Task.CompletedTask;
        });
```

### External Service Resilience
- Wrap all external service calls with circuit breakers
- Implement timeout policies for external calls
- Use bulkhead pattern for resource isolation
- Implement service health checks
- Add circuit breaker state monitoring
- Provide graceful degradation when services fail

#### Service Integration Example
```csharp
public class ResilientCacheService : ICacheService
{
    private readonly ICacheService _innerCacheService;
    private readonly IAsyncPolicy _circuitBreakerPolicy;
    private readonly IAsyncPolicy _retryPolicy;
    private readonly ILogger<ResilientCacheService> _logger;

    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _circuitBreakerPolicy
                .WrapAsync(_retryPolicy)
                .ExecuteAsync(async () => 
                {
                    return await _innerCacheService.GetAsync<T>(key, cancellationToken);
                });
        }
        catch (BrokenCircuitException)
        {
            _logger.LogWarning("Circuit breaker is open, using fallback");
            return default(T);
        }
    }
}
```

### Resilience Testing
- Test circuit breaker state transitions
- Test retry logic with various failure scenarios
- Test fallback mechanisms
- Test cascading failure prevention
- Test circuit breaker recovery
- Load test resilience patterns

---

## Rate Limiting Guidelines

### Rate Limiting Implementation
- Implement rate limiting at middleware level
- Use distributed rate limiting with Redis for multi-instance deployments
- Configure different rate limits per user role
- Add rate limit headers to all responses
- Return 429 status code when limits are exceeded
- Log rate limiting violations for monitoring

#### Rate Limiting Configuration
```csharp
// Configure rate limiting rules
services.AddRateLimiter(options =>
{
    options.AddPolicy("AnonymousPolicy", context =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 2
            }));

    options.AddPolicy("AuthenticatedPolicy", context =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: context.User.FindFirst("sub")?.Value ?? "anonymous",
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 1000,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 2
            }));
});
```

#### Rate Limiting Middleware
```csharp
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimiter _rateLimiter;

    public async Task InvokeAsync(HttpContext context)
    {
        var permit = await _rateLimiter.AttemptAcquireAsync(context);
        
        if (permit.IsAcquired)
        {
            using (permit)
            {
                await _next(context);
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Rate limit exceeded");
        }
    }
}
```

#### Rate Limit Headers
- Add `X-RateLimit-Limit`: Maximum requests allowed
- Add `X-RateLimit-Remaining`: Remaining requests
- Add `X-RateLimit-Reset`: When the limit resets
- Add `X-RateLimit-Reset-After`: Seconds until reset
- Add `Retry-After`: Seconds to wait before retry

### Rate Limiting Best Practices
- Use sliding window algorithm for accurate rate limiting
- Implement different limits for different endpoint types
- Consider resource cost when setting limits
- Provide clear error messages when limits are exceeded
- Monitor rate limiting violations and patterns
- Adjust rate limits based on usage patterns

#### Distributed Rate Limiting with Redis
```csharp
public class RedisRateLimiter
{
    private readonly IConnectionMultiplexer _redis;

    public async Task<bool> IsAllowedAsync(string key, int limit, TimeSpan period)
    {
        var db = _redis.GetDatabase();
        var luaScript = @"
            local current = redis.call('INCR', KEYS[1])
            if tonumber(current) == 1 then
                redis.call('EXPIRE', KEYS[1], ARGV[1])
            end
            return tonumber(current) <= tonumber(ARGV[2])
        ";
        
        var result = await db.ScriptEvaluateAsync(
            luaScript,
            new RedisKey[] { key },
            new RedisValue[] { period.TotalSeconds, limit });
        
        return (bool)result;
    }
}
```

### Rate Limiting Testing
- Test rate limiting per user role
- Test rate limiting per endpoint
- Test distributed rate limiting across instances
- Test rate limit header accuracy
- Test rate limit exceeded responses
- Performance test rate limiting overhead

---

## Testing Guidelines

### Unit Testing
- Test business logic in isolation
- Mock external dependencies
- Use Arrange-Act-Assert pattern
- Test both success and failure cases
- Keep tests independent and fast

#### Unit Test Example
```csharp
[Fact]
public async Task GetBookAsync_WhenBookExists_ReturnsBook()
{
    // Arrange
    var bookId = "123";
    var expectedBook = new Book { Id = bookId, BookName = "Test Book" };
    _mockRepository.Setup(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(expectedBook);
    
    // Act
    var result = await _bookService.GetBookAsync(bookId);
    
    // Assert
    Assert.Equal(expectedBook, result);
}
```

### Integration Testing
- Test API endpoints end-to-end
- Use in-memory database or test database
- Test database operations
- Test authentication/authorization flows
- Use test data factories

#### Integration Test Example
```csharp
[Fact]
public async Task GetBookEndpoint_WhenBookExists_ReturnsOk()
{
    // Arrange
    var client = _factory.CreateClient();
    var bookId = "123";
    
    // Act
    var response = await client.GetAsync($"/api/Book/{bookId}");
    
    // Assert
    response.EnsureSuccessStatusCode();
    var book = await response.Content.ReadFromJsonAsync<BookDTO>();
    Assert.NotNull(book);
}
```

### Testing Best Practices
- Aim for >80% code coverage
- Test edge cases and boundary conditions
- Keep tests maintainable and readable
- Use descriptive test names
- Avoid testing implementation details

---

## Git Workflow

### Commit Message Guidelines
- Use present tense ("add feature" not "added feature")
- Keep messages concise but descriptive
- Include issue/task reference if applicable
- First line ≤ 50 characters
- Body lines ≤ 72 characters

#### Commit Message Format
```
Add Redis caching service

- Implement ICacheService interface
- Add RedisCacheService implementation
- Update Directors to use caching
- Add cache configuration

Closes #123
```

### Branch Guidelines
- `main`: Production-ready code
- `develop`: Integration branch for features
- `feature/*`: Feature branches
- `bugfix/*`: Bug fix branches
- `hotfix/*`: Emergency production fixes

### Pull Request Guidelines
- Keep PRs focused and small
- Include clear description of changes
- Reference related issues
- Ensure all tests pass
- Request review from appropriate team members

---

## Documentation Standards

### Code Documentation
- Add XML comments for public APIs
- Document complex algorithms
- Explain non-obvious logic
- Keep comments up to date
- Avoid redundant comments

#### XML Documentation Example
```csharp
/// <summary>
/// Gets a book by its unique identifier
/// </summary>
/// <param name="bookId">The unique identifier of the book</param>
/// <param name="cancellationToken">Cancellation token for async operation</param>
/// <returns>The book if found, null otherwise</returns>
/// <exception cref="BookNotFoundException">Thrown when book is not found</exception>
public async Task<Book> GetBookAsync(string bookId, CancellationToken cancellationToken = default)
{
    // Implementation
}
```

### API Documentation
- Keep Swagger/OpenAPI documentation current
- Add descriptions for endpoints
- Document request/response schemas
- Include example values
- Document authentication requirements

### README Updates
- Update README.md for significant changes
- Document new features
- Update setup instructions
- Add troubleshooting information
- Keep version information current

---

## Performance Guidelines

### General Performance
- Use async/await for I/O operations
- Avoid unnecessary allocations
- Use object pooling for expensive objects
- Cache frequently accessed data
- Optimize database queries

### Database Performance
- Use appropriate indexes
- Avoid N+1 query problems
- Use pagination for large result sets
- Consider read replicas for scaling
- Monitor query performance

### Memory Management
- Dispose of IDisposable objects
- Use `using` statements for resources
- Avoid memory leaks in event handlers
- Monitor memory usage
- Use appropriate collection types

---

## Security Guidelines

### Authentication and Authorization
- Implement proper authentication
- Use role-based authorization
- Validate user input
- Use parameterized queries
- Implement rate limiting

### Data Protection
- Never log sensitive data
- Encrypt sensitive data at rest
- Use HTTPS in production
- Validate and sanitize input
- Implement proper CORS policies

### Configuration Security
- Never commit secrets
- Use environment variables for secrets
- Use secure configuration providers
- Rotate credentials regularly
- Audit configuration changes

---

## Debugging and Troubleshooting

### Debugging Tips
- Use breakpoints strategically
- Inspect variable values
- Use logging for troubleshooting
- Test in isolation
- Reproduce issues consistently

### Common Issues
- **Database connection issues**: Check connection strings, network connectivity
- **Cache issues**: Verify Redis connectivity, check cache keys
- **Authentication issues**: Validate tokens, check Auth0 configuration
- **Performance issues**: Profile code, check database queries, monitor cache hit rate

### Logging Guidelines
- Use structured logging
- Include correlation IDs
- Log at appropriate levels
- Avoid logging sensitive data
- Log important business events

---

## Build and Deployment

### Build Process
```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Create release build
dotnet build --configuration Release
```

### Environment Configuration
- Use appsettings.json for base configuration
- Use environment-specific files (appsettings.Development.json)
- Override with environment variables
- Secure sensitive configuration
- Document configuration options

### Deployment Checklist
- [ ] All tests pass
- [ ] Configuration updated for target environment
- [ ] Database migrations applied
- [ ] Health checks pass
- [ ] Monitoring configured
- [ ] Documentation updated

---

## Code Review Guidelines

### Review Checklist
- [ ] Code follows project conventions
- [ ] Appropriate error handling
- [ ] No security vulnerabilities
- [ ] Tests included and passing
- [ ] Documentation updated
- [ ] Performance considerations addressed
- [ ] No breaking changes (or documented)

### Review Process
- Be constructive and respectful
- Focus on code quality and best practices
- Ask questions for clarification
- Suggest improvements
- Approve when satisfied

---

## Continuous Improvement

### Regular Tasks
- Update dependencies regularly
- Refactor code for better maintainability
- Improve test coverage
- Update documentation
- Monitor performance metrics

### Learning Resources
- Stay current with .NET best practices
- Follow industry standards
- Learn from open-source projects
- Attend conferences and webinars
- Share knowledge with team

---

**These guidelines should be reviewed and updated regularly as the project evolves and team needs change.**
