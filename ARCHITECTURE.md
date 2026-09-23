# Architecture Documentation - Core API Clean Additional Service

This document provides a comprehensive overview of the current architecture and the target architecture after implementing the planned enhancements (Redis, RabbitMQ, Auth0).

## Current Architecture

### High-Level Overview

The current implementation follows Clean Architecture principles with clear separation of concerns across three main layers:

```
┌─────────────────────────────────────────────────────────────┐
│                    API Layer (Presentation)                   │
│  ┌──────────────────┐  ┌──────────────────┐  ┌─────────────┐│
│  │  BookController  │  │ PersonController │  │PingController││
│  └──────────────────┘  └──────────────────┘  └─────────────┘│
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                 Director Layer (Business Logic)              │
│  ┌──────────────────┐  ┌──────────────────┐                │
│  │  BookDirector    │  │  PersonDirector  │                │
│  └──────────────────┘  └──────────────────┘                │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│              Repository Layer (Data Access)                  │
│  ┌──────────────────┐  ┌──────────────────┐                │
│  │ BookRepository   │  │ PersonRepository │                │
│  └──────────────────┘  └──────────────────┘                │
│  ┌──────────────────────────────────────────┐              │
│  │        Unit of Work (Transactions)       │              │
│  └──────────────────────────────────────────┘              │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│              Infrastructure Layer (Database)                 │
│  ┌──────────────────────────────────────────┐              │
│  │      SQLite Database (Entity Framework)   │              │
│  └──────────────────────────────────────────┘              │
└─────────────────────────────────────────────────────────────┘
```

### Component Details

#### API Layer (CoreAPICleanAdditionalService)
- **Controllers**: Handle HTTP requests, routing, and response formatting
- **DTOs**: Data Transfer Objects for API communication
- **Dependency Injection**: Service registration and configuration
- **Middleware**: CORS, Swagger, routing

#### Director Layer (CoreLibraryCleanAdditionalService/Director)
- **BookDirector**: Orchestrates book-related business logic
- **PersonDirector**: Orchestrates person-related business logic
- **Responsibilities**: 
  - Coordinate repository operations
  - Implement business rules
  - Handle data transformation
  - Manage transactions

#### Repository Layer (CoreLibraryCleanAdditionalService/Repository)
- **Generic Repository Interface**: `IEntityGenericRepository<T>`
- **Concrete Implementations**: 
  - `EntityFrameworkBookRepository`
  - `EntityFrameworkPersonRepository`
- **Unit of Work**: `IUnitOfWork` for transaction management
- **DbContext**: `SqlDataBaseDataContext` for EF Core operations

#### Infrastructure Layer
- **Database**: SQLite with Entity Framework Core
- **ORM**: Entity Framework Core 10.0.11
- **Logging**: Serilog
- **Configuration**: appsettings.json

### Data Flow

#### Read Operation Flow
```
HTTP Request → Controller → Director → Repository → DbContext → Database
                                      ↓
                                 Entity Mapping
                                      ↓
                                 DTO Transformation
                                      ↓
HTTP Response ← Controller ← Director ← Repository ← Data
```

#### Write Operation Flow
```
HTTP Request → Controller → Director → Repository → DbContext → Database
                                      ↓
                                 Unit of Work Commit
                                      ↓
                                 Transaction Management
                                      ↓
HTTP Response ← Controller ← Director ← Result
```

### Current Technology Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| Framework | .NET | 10.0 |
| Database | SQLite | EF Core 10.0.11 |
| ORM | Entity Framework Core | 10.0.11 |
| Logging | Serilog | 4.4.0 |
| API Documentation | Swashbuckle | 10.2.3 |
| JSON | Newtonsoft.Json | 13.0.4 |

### Current Limitations

1. **No Caching**: Every request hits the database
2. **No Messaging**: Synchronous processing only
3. **No Authentication**: All endpoints are public
4. **SQLite Database**: Not suitable for production scaling
5. **Limited Error Handling**: Basic error handling
6. **No Monitoring**: Limited observability
7. **No Rate Limiting**: Vulnerable to abuse

---

## Target Architecture

### High-Level Overview

The target architecture maintains Clean Architecture principles while adding cross-cutting concerns for caching, messaging, security, resilience, and standardized responses:

```
┌─────────────────────────────────────────────────────────────┐
│                    API Layer (Presentation)                   │
│  ┌──────────────────┐  ┌──────────────────┐  ┌─────────────┐│
│  │  BookController  │  │ PersonController │  │PingController││
│  │   [Authorize]    │  │   [Authorize]    │  │             ││
│  │   [RateLimit]   │  │   [RateLimit]    │  │             ││
│  └──────────────────┘  └──────────────────┘  └─────────────┘│
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│              Response & Correlation Layer                    │
│  ┌──────────────────────────────────────────┐              │
│  │   ApiResponse<T> Wrapper                 │              │
│  │   Correlation ID Middleware              │              │
│  │   Error Handling Middleware               │              │
│  │   Pagination Metadata                     │              │
│  └──────────────────────────────────────────┘              │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│            Rate Limiting & Resilience Layer                  │
│  ┌──────────────────┐  ┌──────────────────┐  ┌───────────┐│
│  │  Rate Limiting   │  │  Circuit Breaker │  │ Retry      ││
│  │  Middleware      │  │  Policies        │  │ Policies   ││
│  └──────────────────┘  └──────────────────┘  └───────────┘│
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│              Authentication & Authorization Layer           │
│  ┌──────────────────────────────────────────┐              │
│  │   JWT Bearer Authentication (Auth0)       │              │
│  │   Role-Based Authorization Policies       │              │
│  └──────────────────────────────────────────┘              │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                 Director Layer (Business Logic)              │
│  ┌──────────────────┐  ┌──────────────────┐                │
│  │  BookDirector    │  │  PersonDirector  │                │
│  │  + Cache Layer   │  │  + Cache Layer   │                │
│  │  + Messaging     │  │  + Messaging     │                │
│  └──────────────────┘  └──────────────────┘                │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│              Cross-Cutting Services Layer                    │
│  ┌──────────────────┐  ┌──────────────────┐  ┌───────────┐│
│  │  Redis Cache     │  │  RabbitMQ        │  │ Serilog    ││
│  │  Service         │  │  Publisher       │  │ Logging    ││
│  │  (Circuit Brkr)  │  │  (Circuit Brkr)  │  │            ││
│  └──────────────────┘  └──────────────────┘  └───────────┘│
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│              Repository Layer (Data Access)                  │
│  ┌──────────────────┐  ┌──────────────────┐                │
│  │ BookRepository   │  │ PersonRepository │                │
│  └──────────────────┘  └──────────────────┘                │
│  ┌──────────────────────────────────────────┐              │
│  │        Unit of Work (Transactions)       │              │
│  └──────────────────────────────────────────┘              │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│              Infrastructure Layer (Database)                 │
│  ┌──────────────────────────────────────────┐              │
│  │  PostgreSQL/SQL Server (Production)      │              │
│  │  SQLite (Development)                     │              │
│  └──────────────────────────────────────────┘              │
└─────────────────────────────────────────────────────────────┘
```

### Enhanced Component Details

#### API Layer Enhancements
- **Authentication**: JWT Bearer authentication with Auth0
- **Authorization**: Role-based and permission-based policies
- **API Versioning**: Support for v1 (legacy) and v2 (authenticated)
- **Rate Limiting**: Per-endpoint rate limiting (future enhancement)

#### Director Layer Enhancements
- **Cache Integration**: Read-through and write-through caching
- **Message Publishing**: Event publishing for entity operations
- **Business Logic**: Enhanced with caching and messaging considerations

#### Cross-Cutting Services Layer
- **Redis Cache Service**: Distributed caching layer
- **RabbitMQ Publisher**: Message broker integration
- **Enhanced Logging**: Structured logging with correlation IDs
- **Health Checks**: Comprehensive health monitoring

#### Infrastructure Layer Enhancements
- **Database**: Support for PostgreSQL/SQL Server (production)
- **Connection Management**: Connection pooling and optimization
- **Monitoring**: Performance metrics and observability

### Enhanced Data Flow

#### Read Operation Flow (with Cache)
```
HTTP Request → Controller → Director → Cache Check
                                      ↓
                                 Cache Hit?
                                      ↓
                            Yes → Return Cached Data
                                      ↓
                            No → Repository → Database
                                      ↓
                                 Cache Update
                                      ↓
                                 DTO Transformation
                                      ↓
HTTP Response ← Controller ← Director ← Data
```

#### Write Operation Flow (with Cache and Messaging)
```
HTTP Request → Controller → Director → Repository → Database
                                      ↓
                                 Cache Invalidation
                                      ↓
                                 Message Publishing
                                      ↓
                                 Unit of Work Commit
                                      ↓
HTTP Response ← Controller ← Director ← Result
```

#### Authentication Flow
```
HTTP Request → JWT Validation → Claims Extraction
                                      ↓
                                 Authorization Check
                                      ↓
                                 User Context Setup
                                      ↓
                                 Controller Execution
                                      ↓
HTTP Response ← Response ← Result
```

### Target Technology Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| Framework | .NET | 10.0 |
| Database | PostgreSQL/SQL Server | EF Core 10.0.11 |
| ORM | Entity Framework Core | 10.0.11 |
| Cache | Redis | StackExchange.Redis |
| Messaging | RabbitMQ | RabbitMQ.Client |
| Authentication | Auth0 | OAuth2/OIDC |
| Resilience | Polly | Latest |
| Rate Limiting | AspNetCoreRateLimit | Latest |
| Logging | Serilog | 4.4.0 |
| API Documentation | Swashbuckle | 10.2.3 |
| JSON | Newtonsoft.Json | 13.0.4 |

### Architecture Principles

#### Clean Architecture Compliance
- **Dependency Rule**: Dependencies point inward
- **Interface Segregation**: Specific interfaces for specific needs
- **Single Responsibility**: Each component has one clear purpose
- **Open/Closed**: Open for extension, closed for modification

#### Cross-Cutting Concerns
- **Caching**: Implemented as a cross-cutting service
- **Messaging**: Event-driven architecture support
- **Security**: Authentication and authorization as middleware
- **Logging**: Structured logging throughout the application

#### Scalability Considerations
- **Stateless API**: Supports horizontal scaling
- **Distributed Cache**: Redis for multi-instance caching
- **Message Broker**: Asynchronous processing capabilities
- **Database**: Production-ready database options

---

## Detailed Component Architecture

### Cache Layer Architecture

#### Interface Design
```csharp
public interface ICacheService
{
    Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}
```

#### Implementation Strategy
- **Redis Service**: Concrete implementation using StackExchange.Redis
- **Fallback Strategy**: Fallback to database if cache unavailable
- **Serialization**: JSON serialization for complex objects
- **Key Management**: Hierarchical key structure for organization

#### Cache Patterns
- **Read-Through**: Cache on first read, return cached on subsequent reads
- **Write-Through**: Update cache when data is modified
- **Cache-Aside**: Application manages cache explicitly
- **Time-Based Expiration**: Configurable TTL per data type

### Messaging Layer Architecture

#### Message Contracts
```csharp
public interface IMessage
{
    string MessageType { get; }
    DateTime Timestamp { get; }
    string CorrelationId { get; }
}

public class BookCreatedMessage : IMessage
{
    public string BookId { get; set; }
    public BookDTO BookData { get; set; }
    public string MessageType => "BookCreated";
    public DateTime Timestamp { get; set; }
    public string CorrelationId { get; set; }
}
```

#### Publisher Design
```csharp
public interface IMessagePublisher
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) 
        where TMessage : IMessage;
    Task PublishAsync<TMessage>(IEnumerable<TMessage> messages, CancellationToken cancellationToken = default) 
        where TMessage : IMessage;
}
```

#### RabbitMQ Topology
- **Exchange**: Topic exchange for flexible routing
- **Queues**: Separate queues per message type
- **Routing Keys**: Structured routing for message categorization
- **Dead-Letter Queue**: For failed message handling

### Authentication Layer Architecture

#### JWT Validation Flow
```
Request → JWT Bearer Middleware → Token Validation → Claims Extraction → User Context
                                                                                   ↓
                                                                              Authorization
                                                                                   ↓
                                                                              Controller
```

#### Authorization Policies
```csharp
// Policy definitions
services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
    options.AddPolicy("UserAccess", policy => policy.RequireRole("user", "admin"));
    options.AddPolicy("ReadAccess", policy => policy.RequireClaim("permission", "read:books"));
});
```

#### User Context Service
```csharp
public interface IUserContextService
{
    string UserId { get; }
    string UserName { get; }
    IEnumerable<string> Roles { get; }
    IEnumerable<string> Permissions { get; }
    bool IsInRole(string role);
    bool HasPermission(string permission);
}
```

### Response Model Architecture

#### Response Wrapper Design
```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
    public string RequestId { get; set; }
    public PaginationMetadata Pagination { get; set; }
}

public class ApiErrorResponse
{
    public bool Success { get; set; }
    public ErrorDetail Error { get; set; }
    public DateTime Timestamp { get; set; }
    public string RequestId { get; set; }
    public string Path { get; set; }
}

public class PaginationMetadata
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}
```

#### Correlation ID Middleware
```csharp
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() 
                          ?? Guid.NewGuid().ToString();
        
        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers["X-Correlation-ID"] = correlationId;
        
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}
```

#### Response Wrapper Filter
```csharp
public class ApiResponseFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult objectResult)
        {
            var response = new ApiResponse<object>
            {
                Success = context.Exception == null,
                Data = objectResult.Value,
                Timestamp = DateTime.UtcNow,
                RequestId = context.HttpContext.Items["CorrelationId"]?.ToString()
            };
            
            context.Result = new ObjectResult(response);
        }
    }
}
```

### Circuit Breaker Architecture

#### Circuit Breaker Policy Design
```csharp
public interface ICircuitBreakerService
{
    Task<T> ExecuteAsync<T>(string serviceKey, Func<Task<T>> action);
    Task ExecuteAsync(string serviceKey, Func<Task> action);
    CircuitBreakerState GetState(string serviceKey);
}

public enum CircuitBreakerState
{
    Closed,
    Open,
    HalfOpen
}
```

#### Polly Circuit Breaker Implementation
```csharp
public static class CircuitBreakerPolicies
{
    public static IAsyncPolicy<T> CreateCircuitBreakerPolicy<T>(
        int exceptionsAllowedBeforeBreaking,
        TimeSpan durationOfBreak)
    {
        return Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: exceptionsAllowedBeforeBreaking,
                durationOfBreak: durationOfBreak,
                onBreak: (exception, breakDelay) => 
                {
                    // Log circuit breaker open
                },
                onReset: () => 
                {
                    // Log circuit breaker reset
                },
                onHalfOpen: () => 
                {
                    // Log circuit breaker half-open
                });
    }
}
```

#### Circuit Breaker Service Integration
```csharp
public class RedisCacheServiceWithCircuitBreaker : ICacheService
{
    private readonly ICacheService _innerCacheService;
    private readonly IAsyncPolicy _circuitBreakerPolicy;

    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        return await _circuitBreakerPolicy.ExecuteAsync(async () => 
        {
            return await _innerCacheService.GetAsync<T>(key, cancellationToken);
        });
    }
}
```

### Rate Limiting Architecture

#### Rate Limiting Strategy
```csharp
public interface IRateLimitService
{
    Task<bool> IsAllowedAsync(string key, TimeSpan period, int limit);
    Task<RateLimitInfo> GetRateLimitInfoAsync(string key);
}

public class RateLimitInfo
{
    public int Limit { get; set; }
    public int Remaining { get; set; }
    public DateTime Reset { get; set; }
    public TimeSpan RetryAfter { get; set; }
}
```

#### Rate Limiting Middleware
```csharp
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimitService _rateLimitService;

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = context.User.FindFirst("sub")?.Value ?? 
                    context.Connection.RemoteIpAddress?.ToString();
        
        var rateLimitKey = $"rate_limit:{userId}";
        var isAllowed = await _rateLimitService.IsAllowedAsync(
            rateLimitKey, 
            TimeSpan.FromMinutes(1), 
            100);

        if (!isAllowed)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Rate limit exceeded");
            return;
        }

        await _next(context);
    }
}
```

#### Distributed Rate Limiting with Redis
```csharp
public class RedisRateLimitService : IRateLimitService
{
    private readonly IConnectionMultiplexer _redis;

    public async Task<bool> IsAllowedAsync(string key, TimeSpan period, int limit)
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

---

## Deployment Architecture

### Development Environment
```
┌─────────────────────────────────────────────────────────────┐
│                    Development Machine                        │
│  ┌──────────────────┐  ┌──────────────────┐  ┌─────────────┐│
│  │  API Application │  │  Local Redis     │  │Local RabbitMQ││
│  │  (dotnet run)    │  │  (Docker)        │  │  (Docker)   ││
│  └──────────────────┘  └──────────────────┘  └─────────────┘│
│  ┌──────────────────────────────────────────┐              │
│  │         SQLite Database (local)          │              │
│  └──────────────────────────────────────────┘              │
└─────────────────────────────────────────────────────────────┘
```

### Production Environment
```
                    ┌─────────────────────────────────────┐
                    │           Load Balancer              │
                    │         (HTTPS/TLS Termination)      │
                    └─────────────────────────────────────┘
                                      ↓
        ┌─────────────────────────────┼─────────────────────────────┐
        ↓                             ↓                             ↓
┌───────────────┐           ┌───────────────┐           ┌───────────────┐
│  API Instance │           │  API Instance │           │  API Instance │
│      #1       │           │      #2       │           │      #3       │
└───────────────┘           └───────────────┘           └───────────────┘
        ↓                             ↓                             ↓
        └─────────────────────────────┼─────────────────────────────┘
                                      ↓
                    ┌─────────────────────────────────────┐
                    │          Cross-Cutting Services       │
                    │  ┌──────────┐  ┌──────────┐  ┌──────┐│
                    │  │  Redis   │  │ RabbitMQ │  │ Auth0││
                    │  │  Cluster │  │  Cluster │  │      ││
                    │  └──────────┘  └──────────┘  └──────┘│
                    └─────────────────────────────────────┘
                                      ↓
                    ┌─────────────────────────────────────┐
                    │     Database Cluster (Primary/Replica)│
                    │           PostgreSQL/SQL Server       │
                    └─────────────────────────────────────┘
```

### Security Architecture
```
┌─────────────────────────────────────────────────────────────┐
│                     Security Layers                           │
│  ┌──────────────────────────────────────────┐              │
│  │  Network Security (VPC, Firewalls, WAF)   │              │
│  └──────────────────────────────────────────┘              │
│  ┌──────────────────────────────────────────┐              │
│  │  Transport Security (HTTPS/TLS)          │              │
│  └──────────────────────────────────────────┘              │
│  ┌──────────────────────────────────────────┐              │
│  │  Authentication (JWT, Auth0)              │              │
│  └──────────────────────────────────────────┘              │
│  ┌──────────────────────────────────────────┐              │
│  │  Authorization (RBAC, Permissions)       │              │
│  └──────────────────────────────────────────┘              │
│  ┌──────────────────────────────────────────┐              │
│  │  Application Security (Input Validation) │              │
│  └──────────────────────────────────────────┘              │
│  ┌──────────────────────────────────────────┐              │
│  │  Data Security (Encryption at Rest)       │              │
│  └──────────────────────────────────────────┘              │
└─────────────────────────────────────────────────────────────┘
```

---

## Data Architecture

### Database Schema (Current)
```sql
-- Books Table
CREATE TABLE Books (
    Id TEXT PRIMARY KEY,
    personId TEXT,
    bookCategory TEXT NOT NULL,
    bookName TEXT NOT NULL,
    edition TEXT,
    image TEXT,
    price REAL,
    dateCreated DATETIME,
    FOREIGN KEY (personId) REFERENCES Persons(Id)
);

-- Persons Table
CREATE TABLE Persons (
    Id TEXT PRIMARY KEY,
    firstName TEXT NOT NULL,
    lastName TEXT NOT NULL,
    rank INTEGER,
    category TEXT,
    dateOfBirth DATETIME,
    isPlaySports INTEGER,
    dateCreated DATETIME
);
```

### Database Schema (Target - PostgreSQL)
```sql
-- Books Table
CREATE TABLE books (
    id UUID PRIMARY KEY,
    person_id UUID REFERENCES persons(id),
    book_category VARCHAR(100) NOT NULL,
    book_name VARCHAR(255) NOT NULL,
    edition VARCHAR(50),
    image TEXT,
    price DECIMAL(10,2),
    date_created TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_book_category (book_category),
    INDEX idx_person_id (person_id)
);

-- Persons Table
CREATE TABLE persons (
    id UUID PRIMARY KEY,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    rank INTEGER,
    category VARCHAR(50),
    date_of_birth DATE,
    is_play_sports BOOLEAN DEFAULT FALSE,
    date_created TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_person_category (category),
    INDEX idx_person_name (first_name, last_name)
);
```

### Cache Data Structure
```
Redis Keys:
- book:{id} → BookDTO (JSON)
- book:all → List<BookDTO> (JSON)
- book:search:{query} → List<BookDTO> (JSON)
- person:{id} → PersonDTO (JSON)
- person:all → List<PersonDTO> (JSON)
- person:search:{query} → List<PersonDTO> (JSON)

TTL Configuration:
- Individual entities: 60 minutes
- Lists/Queries: 30 minutes
- Search results: 15 minutes
```

### Message Schema
```json
{
  "messageType": "BookCreated",
  "timestamp": "2024-01-01T00:00:00Z",
  "correlationId": "guid",
  "bookId": "guid",
  "bookData": {
    "id": "guid",
    "personId": "guid",
    "bookCategory": "string",
    "bookName": "string",
    "edition": "string",
    "image": "string",
    "price": 0.0,
    "dateCreated": "2024-01-01T00:00:00Z"
  }
}
```

---

## Integration Patterns

### Cache Integration Pattern
```csharp
public async Task<BookDTO> GetBookAsync(string bookId, CancellationToken cancellationToken)
{
    // Try cache first
    var cacheKey = $"book:{bookId}";
    var cachedBook = await _cacheService.GetAsync<BookDTO>(cacheKey, cancellationToken);
    
    if (cachedBook != null)
    {
        return cachedBook;
    }
    
    // Fallback to database
    var book = await _bookRepository.GetByIdAsync(bookId, cancellationToken);
    
    // Update cache
    if (book != null)
    {
        await _cacheService.SetAsync(cacheKey, book, TimeSpan.FromMinutes(60), cancellationToken);
    }
    
    return book;
}
```

### Message Publishing Pattern
```csharp
public async Task<BookDTO> CreateBookAsync(BookCreateDTO bookDto, CancellationToken cancellationToken)
{
    // Create book
    var book = await _bookRepository.AddAsync(bookDto, cancellationToken);
    await _unitOfWork.CommitAsync(cancellationToken);
    
    // Invalidate cache
    await _cacheService.RemoveByPatternAsync("book:*", cancellationToken);
    
    // Publish message
    var message = new BookCreatedMessage
    {
        BookId = book.Id,
        BookData = book,
        Timestamp = DateTime.UtcNow,
        CorrelationId = Guid.NewGuid().ToString()
    };
    await _messagePublisher.PublishAsync(message, cancellationToken);
    
    return book;
}
```

### Authorization Pattern
```csharp
[Authorize(Policy = "AdminOnly")]
[HttpDelete("{bookId}")]
public async Task<long> Delete(string bookId)
{
    // Only accessible to users with admin role
    var result = await bookDirector.DeleteEntityByIdAsync(bookId, default);
    return result;
}

[Authorize(Policy = "UserAccess")]
[HttpGet]
public async Task<IEnumerable<BookDTO>> Get()
{
    // Accessible to users and admins
    IEnumerable<BookDTO> books = await bookDirector.GetEntitiesAsync(default);
    return books;
}
```

---

## Performance Considerations

### Caching Strategy
- **Read-heavy workloads**: Cache frequently accessed data
- **Write-heavy workloads**: Careful cache invalidation
- **Cache warming**: Pre-populate cache for known hot data
- **Cache stampede prevention**: Use cache locks or refresh-ahead

### Database Optimization
- **Connection pooling**: Optimize pool size and timeout
- **Query optimization**: Use appropriate indexes
- **N+1 prevention**: Use eager loading where appropriate
- **Batch operations**: Batch inserts/updates where possible

### Messaging Performance
- **Async publishing**: Don't block API operations
- **Batch publishing**: Batch messages when possible
- **Connection reuse**: Reuse RabbitMQ connections
- **Error handling**: Don't fail API operations on messaging errors

---

## Monitoring and Observability

### Health Checks
```csharp
// Health check endpoints
- /health - Overall application health
- /health/cache - Redis connectivity
- /health/messaging - RabbitMQ connectivity
- /health/database - Database connectivity
- /health/auth - Auth0 connectivity
- /health/circuit-breaker - Circuit breaker states
- /health/rate-limiting - Rate limiting service health
```

### Metrics Collection
- **API Metrics**: Request count, response time, error rate
- **Cache Metrics**: Hit rate, miss rate, eviction rate
- **Messaging Metrics**: Publish rate, error rate, queue depth
- **Database Metrics**: Query time, connection pool usage
- **Authentication Metrics**: Login success/failure rate
- **Circuit Breaker Metrics**: State transitions, success/failure rates, breaker duration
- **Rate Limiting Metrics**: Violations per endpoint, violations per user, active rate limits

### Logging Strategy
- **Structured Logging**: JSON format with correlation IDs
- **Log Levels**: Debug, Information, Warning, Error, Critical
- **Log Aggregation**: Centralized logging service
- **Sensitive Data**: Never log passwords, tokens, or PII

---

## Migration Strategy

### Phase 0 Migration (API Response Model)
- Breaking change requiring API versioning
- Parallel deployment of v1 (legacy) and v2 (new response format)
- Migration period for existing clients
- Comprehensive testing and validation
- Client migration guide

### Phase 1 Migration (Redis)
- No breaking changes
- Feature flag for cache enable/disable
- Gradual rollout with monitoring
- Performance validation

### Phase 2 Migration (RabbitMQ)
- No breaking changes
- Message publishing as background process
- Gradual rollout with monitoring
- Message delivery validation

### Phase 3 Migration (Auth0)
- Breaking change requiring API versioning
- Parallel deployment of v2 and v3
- Migration period for existing clients
- Comprehensive testing and validation

### Phase 4 Migration (Circuit Breaker)
- No breaking changes
- Gradual rollout with monitoring
- Circuit breaker state validation
- Fallback mechanism testing

### Phase 5 Migration (Rate Limiting)
- No breaking changes
- Gradual rollout with monitoring
- Rate limit validation
- User communication

---

## Future Enhancements

### Potential Future Features
- **API Rate Limiting**: Per-endpoint rate limiting
- **GraphQL**: Alternative to REST API
- **Real-time Updates**: SignalR for real-time notifications
- **File Upload**: S3 integration for file storage
- **Advanced Search**: Elasticsearch integration
- **API Gateway**: Centralized API management
- **Service Mesh**: Istio or Linkerd for microservices

### Scalability Path
- **Horizontal Scaling**: Multiple API instances
- **Database Sharding**: Horizontal database scaling
- **Read Replicas**: Read-only database replicas
- **CDN Integration**: Static content delivery
- **Edge Computing**: Edge API endpoints

---

**This architecture document should be updated as the system evolves and new requirements are introduced.**
