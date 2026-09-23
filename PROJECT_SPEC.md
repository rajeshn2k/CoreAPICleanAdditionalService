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

## Planned Enhancements

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
- Rate limiting (future enhancement)

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

## Migration Strategy

### From Current to Enhanced System
1. **Phase 1**: Add Redis cache without breaking changes
2. **Phase 2**: Add RabbitMQ messaging without breaking changes
3. **Phase 3**: Add Auth0 authentication (breaking change - plan API versioning)

### API Versioning Strategy
- Maintain v1 API without authentication during transition
- Introduce v2 API with authentication
- Deprecate v1 after migration period
- Provide migration guide for API consumers

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
- Request/response logging
- Error tracking and alerting

### Metrics
- API response times
- Cache hit/miss ratios
- Database query performance
- Message publishing rates
- Authentication success/failure rates

### Health Checks
- Database connectivity
- Redis connectivity
- RabbitMQ connectivity
- Auth0 connectivity
- Application health endpoint

---

**This specification serves as the foundation for AI-assisted development and should be updated as requirements evolve.**
