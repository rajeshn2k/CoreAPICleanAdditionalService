# Development Guidelines - Core API Clean Additional Service

This document provides comprehensive coding standards, development practices, and guidelines for contributing to this .NET API project.

## Table of Contents
- [Code Style and Conventions](#code-style-and-conventions)
- [Project Structure Guidelines](#project-structure-guidelines)
- [Entity Framework Guidelines](#entity-framework-guidelines)
- [API Development Guidelines](#api-development-guidelines)
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
