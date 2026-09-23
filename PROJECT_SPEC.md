# Project Specification - Core API Clean Additional Service

## Executive Summary

This project is a Clean Architecture .NET 10.0 Web API that provides basic CRUD operations for Book and Person entities. The API is designed with educational purposes in mind and serves as a foundation for learning C# programming and modern API development practices.

## Current Implementation

### Technology Stack
- **Framework**: .NET 10.0
- **Database**: SQLite with Entity Framework Core 10.0.11
- **Architecture**: Clean Architecture with Director pattern
- **Logging**: Serilog 4.4.0
- **API Documentation**: Swagger/OpenAPI with Swashbuckle.AspNetCore 10.2.3
- **JSON Handling**: Newtonsoft.Json 13.0.4

### Project Structure
```
Core.API.Clean.AdditionalService.sln
├── CoreAPICleanAdditionalService/          # API Layer
│   ├── Controllers/                        # REST API endpoints
│   │   ├── BookController.cs              # Book CRUD operations
│   │   ├── PersonController.cs            # Person CRUD operations
│   │   └── PingController.cs              # Health check endpoint
│   ├── Code/                              # Configuration
│   │   └── DependencyInjection.cs         # Service registration
│   ├── Program.cs                         # Application entry point
│   └── appsettings.json                   # Configuration files

└── CoreLibraryCleanAdditionalService/      # Core Business Layer
    ├── Models/                            # Domain models and DTOs
    │   ├── Book.cs, BookDTO.cs, BookCreateDTO.cs
    │   └── Person.cs, PersonDTO.cs, PersonCreateDTO.cs
    ├── Director/                          # Business logic coordinators
    │   ├── BookDirector.cs               # Book business logic
    │   └── PersonDirector.cs             # Person business logic
    ├── Repository/                        # Data access layer
    │   ├── Data/                         # Repository implementations
    │   ├── EF/                           # Entity Framework setup
    │   ├── UnitOfWork/                   # Transaction management
    │   └── SqlDataBaseDataContext.cs     # DbContext
    ├── Mapper/                            # Object mapping
    ├── Messaging/                         # Message publishing (currently empty)
    └── Log/                              # Logging interfaces
```

### Domain Models

#### Book Entity
```csharp
public class Book
{
    public string Id { get; set; }
    public string? personId { get; set; }        // Foreign key to Person
    public string bookCategory { get; set; }     // Book category
    public string bookName { get; set; }         // Book title
    public string edition { get; set; }           // Edition information
    public string image { get; set; }            // Image URL/path
    public double price { get; set; }            // Price
    public DateTime dateCreated { get; set; }    // Creation timestamp
}
```

#### Person Entity
```csharp
public class Person
{
    public string Id { get; set; }
    public string firstName { get; set; }        // First name
    public string lastName { get; set; }         // Last name
    public int rank { get; set; }                // Rank/score
    public string category { get; set; }         // Category classification
    public DateTime dateOfBirth { get; set; }    // Date of birth
    public bool isPlaySports { get; set; }       // Sports participation flag
    public DateTime dateCreated { get; set; }    // Creation timestamp
}
```

### API Endpoints

#### Book Controller (`/api/Book`)
- `GET /api/Book` - Get all books
- `GET /api/Book/{bookId}` - Get book by ID
- `GET /api/Book/SearchByBook/{searchValue}` - Search books by category or name
- `GET /api/Book/SearchByPersonId/{personId}` - Get books by person ID
- `POST /api/Book` - Create a new book
- `POST /api/Book/Many` - Create multiple books
- `PUT /api/Book/{bookId}` - Update a book
- `DELETE /api/Book/{bookId}` - Delete a book
- `DELETE /api/Book/Many` - Delete all books
- `GET /api/Book/LoadAllBookForNewDatabase` - Load all books for database initialization

#### Person Controller (`/api/Person`)
- `GET /api/Person` - Get all persons
- `GET /api/Person/{personId}` - Get person by ID
- `GET /api/Person/SearchByPerson/{searchValue}` - Search persons by category, first name, or last name
- `GET /api/Person/SearchByBookId/{bookId}` - Get persons by book ID
- `POST /api/Person` - Create a new person
- `POST /api/Person/Many` - Create multiple persons
- `PUT /api/Person/{personId}` - Update a person
- `DELETE /api/Person/{personId}` - Delete a person
- `DELETE /api/Person/Many` - Delete all persons
- `GET /api/Person/LoadAllPersonForNewDatabase` - Load all persons for database initialization

#### Ping Controller (`/api/Ping`)
- `GET /api/Ping` - Health check endpoint

### Architecture Patterns

#### Clean Architecture Layers
1. **API Layer** (Controllers): Handles HTTP requests/responses, validation, routing
2. **Director Layer**: Business logic orchestration, coordinates between repositories
3. **Repository Layer**: Data access abstraction, database operations
4. **Domain Layer**: Core business entities and DTOs

#### Design Patterns Used
- **Director Pattern**: Business logic coordinators that orchestrate repository operations
- **Repository Pattern**: Abstraction over data access with generic interfaces
- **Unit of Work Pattern**: Transaction management across multiple repository operations
- **DTO Pattern**: Data transfer objects for API communication
- **Dependency Injection**: Inversion of control for loose coupling

### Current Limitations
- No authentication/authorization mechanism
- No caching layer
- No message publishing/consuming capabilities
- SQLite database (not suitable for production scaling)
- Limited error handling and validation
- No comprehensive test coverage
- No API rate limiting
- No monitoring and observability features
- Inconsistent API response format
- No circuit breaker pattern for external service resilience
- No standardized error response model
- No request/response correlation tracking

## Planned Enhancements

### Phase 0: Standardized API Response Model
**Objective**: Implement consistent API response format across all endpoints with proper error handling, correlation tracking, and standardized structure.

**Requirements**:
- Create unified API response wrapper model
- Implement consistent error response format
- Add request/response correlation ID tracking
- Standardize HTTP status code usage
- Add pagination metadata for list responses
- Implement response envelope with success/error indicators
- Add timestamp and request ID to all responses

**Implementation Approach**:
- Create `ApiResponse<T>` generic wrapper model in CoreLibrary/Models/
- Create `ApiErrorResponse` model for error responses
- Create `PaginationMetadata` model for paginated responses
- Implement correlation ID middleware in API project
- Update all controllers to use standardized response models
- Create response wrapper helper methods
- Add response filters for automatic wrapping

**API Impact**:
- Breaking change: Response structure changes for all endpoints
- Clients will need to update to handle new response format
- Provides consistent structure for all API responses
- Better error handling and debugging capabilities

### Phase 1: Redis Distributed Cache
**Objective**: Implement caching layer to improve performance and reduce database load.

**Requirements**:
- Integrate Redis as distributed cache provider
- Cache frequently accessed data (books, persons)
- Implement cache invalidation strategies
- Support cache expiration policies
- Configure Redis connection settings
- Provide cache statistics and monitoring

**Implementation Approach**:
- Add `StackExchange.Redis` NuGet package
- Create cache service interfaces in CoreLibrary
- Implement Redis cache service in API project
- Update Directors to use caching layer
- Configure cache settings in appsettings.json
- Add cache invalidation on data changes

**API Impact**:
- No breaking changes to existing endpoints
- Improved response times for cached data
- Reduced database load

### Phase 2: RabbitMQ Message Broker
**Objective**: Implement message publishing for event-driven architecture and asynchronous processing.

**Requirements**:
- Integrate RabbitMQ as message broker
- Implement message publishing for entity operations (create, update, delete)
- Define message schemas and contracts
- Configure RabbitMQ connection and exchanges
- Support message serialization/deserialization
- Implement error handling and retry logic
- Add message monitoring capabilities

**Implementation Approach**:
- Add `RabbitMQ.Client` NuGet package
- Define message contracts in CoreLibrary
- Implement `IMessagePublisher` interface (currently `EmptyMessagePublisher`)
- Create RabbitMQ publisher service
- Configure exchanges, queues, and bindings
- Update Directors to publish messages on operations
- Add connection failure handling and reconnection logic

**Message Events**:
- `BookCreated`, `BookUpdated`, `BookDeleted`
- `PersonCreated`, `PersonUpdated`, `PersonDeleted`
- Include entity data and operation metadata

**API Impact**:
- No breaking changes to existing endpoints
- Additional configuration required for RabbitMQ
- Asynchronous message publishing

### Phase 3: Auth0 Authentication and Authorization
**Objective**: Implement secure authentication and role-based authorization using Auth0.

**Requirements**:
- Integrate Auth0 for OAuth2/OIDC authentication
- Implement JWT token validation
- Add role-based authorization (RBAC)
- Secure API endpoints with authentication
- Implement user profile management
- Configure Auth0 application settings
- Add token refresh logic
- Implement authorization policies

**Implementation Approach**:
- Add `Microsoft.AspNetCore.Authentication.JwtBearer` NuGet package
- Configure Auth0 authentication in Program.cs
- Add `[Authorize]` attributes to controllers
- Implement role-based authorization policies
- Configure Auth0 domain, audience, and client ID
- Add user context services
- Implement token validation middleware
- Create user profile management endpoints

**Security Considerations**:
- HTTPS enforcement in production
- Token expiration and refresh
- Secure storage of Auth0 credentials
- Role-based access control for different operations
- Audit logging for authentication events

**API Impact**:
- Breaking change: All endpoints will require authentication
- New endpoints for user management (optional)
- Configuration changes required
- Migration guide for existing API consumers

### Phase 4: Circuit Breaker Pattern Implementation
**Objective**: Implement circuit breaker pattern for external service resilience and fault tolerance.

**Requirements**:
- Implement circuit breaker for external service calls (Redis, RabbitMQ, Auth0)
- Add retry logic with exponential backoff
- Implement fallback mechanisms when services are unavailable
- Monitor circuit breaker state and health
- Configure circuit breaker thresholds and timeouts
- Add circuit breaker metrics and monitoring
- Implement bulkhead pattern for resource isolation

**Implementation Approach**:
- Add `Polly` NuGet package for resilience patterns
- Create circuit breaker policies for each external service
- Implement retry policies with configurable backoff
- Add fallback mechanisms for service failures
- Create circuit breaker state monitoring
- Configure circuit breaker in DependencyInjection.cs
- Add health checks for circuit breaker states
- Implement circuit breaker metrics dashboard

**Circuit Breaker Targets**:
- Redis cache service calls
- RabbitMQ message publishing
- Auth0 authentication token validation
- External API calls (future integrations)

**API Impact**:
- No breaking changes to existing endpoints
- Improved resilience and fault tolerance
- Graceful degradation when external services fail
- Better error handling and recovery

### Phase 5: Rate Limiting Implementation
**Objective**: Implement API rate limiting to prevent abuse, ensure fair usage, and protect system resources.

**Requirements**:
- Implement rate limiting per endpoint and per user
- Support different rate limits for different user roles
- Implement sliding window rate limiting algorithm
- Add rate limit headers to API responses
- Configure rate limit policies in configuration
- Implement distributed rate limiting using Redis
- Add rate limit exceeded error responses
- Monitor rate limiting metrics and violations

**Implementation Approach**:
- Add `AspNetCoreRateLimit` NuGet package
- Configure rate limiting rules in appsettings.json
- Implement distributed rate limiting using Redis
- Add rate limiting middleware to pipeline
- Configure different limits for different endpoints
- Implement per-user and per-IP rate limiting
- Add rate limit headers (X-RateLimit-Limit, X-RateLimit-Remaining, etc.)
- Create rate limit exceeded exception handling
- Add rate limiting metrics and monitoring

**Rate Limiting Strategy**:
- **Anonymous users**: 100 requests per minute
- **Authenticated users**: 1000 requests per minute
- **Admin users**: 5000 requests per minute
- **Write operations**: Stricter limits than read operations
- **Endpoint-specific limits**: Based on resource cost

**API Impact**:
- Non-breaking change with gradual rollout
- Rate limit headers added to all responses
- 429 Too Many Requests status for exceeded limits
- Configuration-driven rate limit policies

## Non-Functional Requirements

### Performance
- Target response time: < 200ms for cached operations
- Target response time: < 500ms for database operations
- Support concurrent users: 100+ (with Redis)
- Database connection pooling optimization

### Scalability
- Horizontal scaling capability (stateless API design)
- Distributed caching for multi-instance deployment
- Message queue for asynchronous processing
- Database migration path from SQLite to production database

### Security
- OWASP compliance for API security
- Input validation and sanitization
- SQL injection prevention (EF Core parameterized queries)
- XSS prevention
- CORS configuration
- Rate limiting for abuse prevention
- Circuit breaker for external service resilience
- Request correlation for security auditing

### Reliability
- Graceful error handling
- Database transaction management
- Connection retry logic for external services
- Health check endpoints
- Logging and monitoring

### Maintainability
- Clean Architecture principles
- Comprehensive documentation
- Code comments for complex logic
- Consistent coding standards
- Test coverage > 80%

## Configuration Requirements

### Application Settings Structure
```json
{
  "ConnectionStrings": {
    "SqliteDBContext": "Data Source=app.db",
    "Redis": "localhost:6379",
    "RabbitMQ": "amqp://localhost:5672"
  },
  "Auth0": {
    "Domain": "your-auth0-domain",
    "Audience": "your-api-identifier",
    "ClientId": "your-client-id"
  },
  "Cache": {
    "DefaultExpirationMinutes": 30,
    "EnableCache": true
  },
  "Messaging": {
    "ExchangeName": "additional-service",
    "QueuePrefix": "additional-service"
  },
  "ApiResponse": {
    "IncludeTimestamp": true,
    "IncludeRequestId": true,
    "DetailedErrors": true
  },
  "CircuitBreaker": {
    "Redis": {
      "ExceptionsAllowedBeforeBreaking": 5,
      "DurationOfBreakInSeconds": 30,
      "RetryCount": 3
    },
    "RabbitMQ": {
      "ExceptionsAllowedBeforeBreaking": 3,
      "DurationOfBreakInSeconds": 60,
      "RetryCount": 5
    },
    "Auth0": {
      "ExceptionsAllowedBeforeBreaking": 5,
      "DurationOfBreakInSeconds": 300,
      "RetryCount": 2
    }
  },
  "RateLimiting": {
    "EnableRateLimiting": true,
    "UseDistributedRateLimiting": true,
    "Rules": {
      "Anonymous": {
        "PerMinute": 100,
        "PerHour": 1000
      },
      "Authenticated": {
        "PerMinute": 1000,
        "PerHour": 10000
      },
      "Admin": {
        "PerMinute": 5000,
        "PerHour": 50000
      }
    }
  },
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:8080"
      }
    }
  }
}
```

## Database Schema Considerations

### Current Schema
- SQLite database with Book and Person tables
- Foreign key relationship: Book.personId → Person.Id
- Automatic database creation on startup

### Future Migration Path
- Migration to PostgreSQL or SQL Server for production
- Index optimization for search operations
- Database backup and restore strategies
- Connection string configuration for different environments

## Testing Strategy

### Unit Tests
- Director layer business logic testing
- Repository layer mocking
- Service layer testing
- Cache service testing
- Message publisher testing

### Integration Tests
- API endpoint testing
- Database integration testing
- Cache integration testing
- Message broker integration testing
- Authentication flow testing

### Performance Tests
- Load testing for API endpoints
- Cache performance measurement
- Database query optimization
- Message publishing throughput

## Deployment Considerations

### Development Environment
- Local SQLite database
- Local Redis instance (optional)
- Local RabbitMQ instance (optional)
- Development Auth0 application

### Production Environment
- PostgreSQL or SQL Server database
- Managed Redis service (Azure Redis, AWS ElastiCache)
- Managed RabbitMQ service (Azure Service Bus, AWS SQS)
- Production Auth0 application
- HTTPS/TLS encryption
- Load balancer configuration
- Health monitoring and alerting

## Success Criteria

### Phase 0 (API Response Model)
- [ ] Unified API response wrapper implemented
- [ ] All endpoints using standardized response format
- [ ] Correlation ID tracking working
- [ ] Error responses standardized
- [ ] Pagination metadata implemented
- [ ] Response times unaffected by new wrapper
- [ ] Client migration guide completed

### Phase 1 (Redis Cache)
- [ ] Redis integration completed
- [ ] Cache hit rate > 70% for frequently accessed data
- [ ] Response time improvement > 50% for cached operations
- [ ] No cache consistency issues
- [ ] Configuration externalized

### Phase 2 (RabbitMQ Messaging)
- [ ] RabbitMQ integration completed
- [ ] Message publishing for all CRUD operations
- [ ] Error handling and retry logic implemented
- [ ] Message serialization/deserialization working
- [ ] Connection failure handling implemented

### Phase 3 (Auth0 Authentication)
- [ ] Auth0 integration completed
- [ ] JWT token validation working
- [ ] Role-based authorization implemented
- [ ] All endpoints secured
- [ ] Token refresh logic implemented
- [ ] Security audit passed

### Phase 4 (Circuit Breaker)
- [ ] Circuit breaker implemented for all external services
- [ ] Retry logic with exponential backoff working
- [ ] Fallback mechanisms functioning
- [ ] Circuit breaker state monitoring in place
- [ ] No cascading failures during service outages
- [ ] Circuit breaker metrics dashboard operational

### Phase 5 (Rate Limiting)
- [ ] Rate limiting implemented for all endpoints
- [ ] Distributed rate limiting using Redis working
- [ ] Per-user and per-IP rate limiting functional
- [ ] Rate limit headers properly set
- [ ] 429 responses for exceeded limits
- [ ] Rate limiting metrics and monitoring in place

## Migration Strategy

### From Current to Enhanced System
1. **Phase 0**: Implement standardized API response model (breaking change - plan API versioning)
2. **Phase 1**: Add Redis cache without breaking changes
3. **Phase 2**: Add RabbitMQ messaging without breaking changes
4. **Phase 3**: Add Auth0 authentication (breaking change - plan API versioning)
5. **Phase 4**: Add circuit breaker pattern without breaking changes
6. **Phase 5**: Add rate limiting without breaking changes

### API Versioning Strategy
- Maintain v1 API without response wrapper during transition
- Introduce v2 API with standardized response model
- Maintain v1 without authentication during Phase 0 transition
- Introduce v3 API with authentication in Phase 3
- Deprecate previous versions after migration periods
- Provide migration guide for API consumers

### API Response Model Structure

#### Success Response
```json
{
  "success": true,
  "data": {
    "id": "123",
    "bookName": "Sample Book",
    "bookCategory": "Fiction",
    "price": 29.99
  },
  "message": "Book retrieved successfully",
  "timestamp": "2024-01-01T00:00:00Z",
  "requestId": "correlation-id-guid",
  "pagination": {
    "currentPage": 1,
    "pageSize": 10,
    "totalItems": 100,
    "totalPages": 10
  }
}
```

#### Error Response
```json
{
  "success": false,
  "error": {
    "code": "BOOK_NOT_FOUND",
    "message": "Book with ID 123 not found",
    "details": "The requested book does not exist in the database",
    "statusCode": 404
  },
  "timestamp": "2024-01-01T00:00:00Z",
  "requestId": "correlation-id-guid",
  "path": "/api/Book/123"
}
```

#### Rate Limit Exceeded Response
```json
{
  "success": false,
  "error": {
    "code": "RATE_LIMIT_EXCEEDED",
    "message": "Rate limit exceeded",
    "details": "Maximum 100 requests per minute allowed",
    "statusCode": 429
  },
  "timestamp": "2024-01-01T00:00:00Z",
  "requestId": "correlation-id-guid",
  "retryAfter": 45
}
```

## Documentation Requirements

### Developer Documentation
- API documentation (Swagger/OpenAPI)
- Architecture documentation
- Setup and installation guide
- Configuration guide
- Development workflow

### User Documentation
- API endpoint reference
- Authentication guide
- Error handling guide
- Rate limiting information
- Best practices

## Monitoring and Observability

### Logging
- Structured logging with Serilog
- Log levels: Debug, Information, Warning, Error, Critical
- Request/response logging with correlation IDs
- Error tracking and alerting
- Circuit breaker state changes logging
- Rate limiting violations logging

### Metrics
- API response times
- Cache hit/miss ratios
- Database query performance
- Message publishing rates
- Authentication success/failure rates
- Circuit breaker state transitions
- Circuit breaker success/failure rates
- Rate limiting violations per endpoint
- Rate limiting violations per user/IP

### Health Checks
- Database connectivity
- Redis connectivity
- RabbitMQ connectivity
- Auth0 connectivity
- Circuit breaker states
- Rate limiting service health
- Application health endpoint

---

**This specification serves as the foundation for AI-assisted development and should be updated as requirements evolve.**
