# Development Roadmap - Core API Clean Additional Service

This roadmap outlines the iterative development plan for enhancing the .NET API with Redis caching, RabbitMQ messaging, and Auth0 authentication.

## Overview

The development will proceed in six distinct phases, each building upon the previous implementation while maintaining Clean Architecture principles and avoiding breaking changes where possible. The new phases address API response standardization, resilience patterns, and security enhancements.

---

## Phase 0: Standardized API Response Model

### Objectives
- Implement consistent API response format across all endpoints
- Add proper error handling and standardized error responses
- Implement request/response correlation tracking
- Add pagination metadata for list responses
- Establish foundation for future monitoring and debugging

### Prerequisites
- Project currently builds and runs successfully
- Understanding of current API response patterns
- API versioning strategy defined (for breaking changes)

### Implementation Steps

#### 0.1 Response Model Design
- [ ] Create `ApiResponse<T>` generic wrapper model in CoreLibrary/Models/
- [ ] Create `ApiErrorResponse` model for error responses
- [ ] Create `PaginationMetadata` model for paginated responses
- [ ] Define error code constants and enumerations
- [ ] Create response model unit tests

#### 0.2 Correlation Tracking
- [ ] Create correlation ID middleware in API project
- [ ] Implement request ID generation
- [ ] Add correlation ID to response headers
- [ ] Implement request-scoped correlation context
- [ ] Add correlation ID to all log entries

#### 0.3 Controller Updates
- [ ] Update all controllers to use `ApiResponse<T>` wrapper
- [ ] Create response wrapper helper methods
- [ ] Update error handling to use `ApiErrorResponse`
- [ ] Add pagination metadata to list endpoints
- [ ] Update XML documentation for changed responses

#### 0.4 Response Filters and Middleware
- [ ] Create response filter for automatic wrapping
- [ ] Implement exception handling middleware
- [ ] Add response formatting middleware
- [ ] Configure response filtering pipeline
- [ ] Add response compression middleware

#### 0.5 API Versioning
- [ ] Implement API versioning in controllers
- [ ] Create v1 endpoints (legacy format)
- [ ] Create v2 endpoints (new response format)
- [ ] Configure versioning strategy
- [ ] Add deprecation headers to v1 endpoints

#### 0.6 Testing and Validation
- [ ] Test all endpoints with new response format
- [ ] Verify correlation ID tracking
- [ ] Test error response handling
- [ ] Validate pagination metadata
- [ ] Performance testing for response wrapping overhead
- [ ] Client migration testing

### Technical Specifications

#### Response Wrapper Model
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
```

#### Error Codes
```csharp
public static class ErrorCodes
{
    public const string VALIDATION_ERROR = "VALIDATION_ERROR";
    public const string NOT_FOUND = "NOT_FOUND";
    public const string UNAUTHORIZED = "UNAUTHORIZED";
    public const string FORBIDDEN = "FORBIDDEN";
    public const string INTERNAL_ERROR = "INTERNAL_ERROR";
    public const string RATE_LIMIT_EXCEEDED = "RATE_LIMIT_EXCEEDED";
    public const string SERVICE_UNAVAILABLE = "SERVICE_UNAVAILABLE";
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
        
        await _next(context);
    }
}
```

### Success Criteria
- [ ] All endpoints using standardized response format
- [ ] Correlation ID tracking working across all requests
- [ ] Error responses consistent and informative
- [ ] Pagination metadata accurate and complete
- [ ] Response wrapping overhead < 5ms
- [ ] API versioning functional
- [ ] Migration guide completed

### Dependencies
- No external dependencies required
- Breaking change: Response structure changes
- Requires API versioning strategy
- Client migration required

### Estimated Effort
- **Development**: 3-4 days
- **Testing**: 2-3 days
- **Documentation**: 1 day
- **Client Migration Support**: 1-2 days

---

## Phase 1: Redis Distributed Cache Integration

### Objectives
- Implement distributed caching layer using Redis
- Improve API response times for frequently accessed data
- Reduce database load for read-heavy operations
- Establish caching patterns for future use

### Prerequisites
- Redis server instance (local or cloud)
- Project currently builds and runs successfully
- Understanding of current data access patterns

### Implementation Steps

#### 1.1 Infrastructure Setup
- [ ] Add `StackExchange.Redis` NuGet package to API project
- [ ] Add `Microsoft.Extensions.Caching.StackExchangeRedis` NuGet package
- [ ] Create Redis configuration section in appsettings.json
- [ ] Add Redis connection string to configuration
- [ ] Set up local Redis instance for development

#### 1.2 Cache Layer Implementation
- [ ] Create `ICacheService` interface in CoreLibrary/Cache/
- [ ] Implement `RedisCacheService` in API project
- [ ] Create cache key management utilities
- [ ] Implement cache serialization/deserialization
- [ ] Add cache expiration strategies
- [ ] Implement cache invalidation methods

#### 1.3 Integration with Business Logic
- [ ] Update `BookDirector` to use caching for Get operations
- [ ] Update `PersonDirector` to use caching for Get operations
- [ ] Implement cache invalidation on Create/Update/Delete operations
- [ ] Add cache statistics tracking
- [ ] Implement cache warming strategies

#### 1.4 Configuration and Dependency Injection
- [ ] Register cache services in DependencyInjection.cs
- [ ] Configure cache settings (expiration, connection)
- [ ] Add health check for Redis connectivity
- [ ] Configure development vs production Redis settings

#### 1.5 Testing and Validation
- [ ] Test cache hit/miss scenarios
- [ ] Verify cache invalidation on data changes
- [ ] Measure performance improvements
- [ ] Test Redis connection failure handling
- [ ] Validate cache consistency

### Technical Specifications

#### Cache Configuration
```json
{
  "Cache": {
    "Enabled": true,
    "RedisConnectionString": "localhost:6379",
    "DefaultExpirationMinutes": 30,
    "BookExpirationMinutes": 60,
    "PersonExpirationMinutes": 60,
    "EnableStatistics": true
  }
}
```

#### Cache Key Patterns
- `book:{id}` - Individual book cache
- `book:all` - All books cache
- `book:search:{query}` - Search results cache
- `person:{id}` - Individual person cache
- `person:all` - All persons cache
- `person:search:{query}` - Search results cache

#### Cache Strategy
- **Read-through**: Cache on first read, return cached data on subsequent reads
- **Write-through**: Update cache when data is modified
- **Cache aside**: Application manages cache explicitly
- **Expiration**: Time-based expiration with configurable TTL

### Success Criteria
- [ ] Cache hit rate > 70% for frequently accessed data
- [ ] Response time improvement > 50% for cached operations
- [ ] No cache consistency issues
- [ ] Graceful degradation when Redis is unavailable
- [ ] Configuration externalized and documented

### Dependencies
- Requires local/remote Redis server
- No breaking changes to existing API
- Can be deployed independently

### Estimated Effort
- **Development**: 2-3 days
- **Testing**: 1-2 days
- **Documentation**: 0.5 day

---

## Phase 2: RabbitMQ Message Broker Integration

### Objectives
- Implement message publishing for event-driven architecture
- Enable asynchronous processing of entity operations
- Establish messaging patterns for future microservices
- Support integration with external systems

### Prerequisites
- RabbitMQ server instance (local or cloud)
- Phase 1 (Redis) completed (optional but recommended)
- Understanding of messaging patterns

### Implementation Steps

#### 2.1 Infrastructure Setup
- [ ] Add `RabbitMQ.Client` NuGet package to API project
- [ ] Add `MassTransit` or `EasyNetQ` for higher-level abstraction (optional)
- [ ] Create RabbitMQ configuration section in appsettings.json
- [ ] Add RabbitMQ connection string to configuration
- [ ] Set up local RabbitMQ instance for development

#### 2.2 Message Contracts
- [ ] Define message contracts in CoreLibrary/Messaging/Contracts/
- [ ] Create `BookCreatedMessage`, `BookUpdatedMessage`, `BookDeletedMessage`
- [ ] Create `PersonCreatedMessage`, `PersonUpdatedMessage`, `PersonDeletedMessage`
- [ ] Define message schemas and validation rules
- [ ] Implement message serialization/deserialization

#### 2.3 Message Publisher Implementation
- [ ] Replace `EmptyMessagePublisher` with `RabbitMQMessagePublisher`
- [ ] Implement connection management and reconnection logic
- [ ] Create exchange and queue topology
- [ ] Implement message publishing with error handling
- [ ] Add retry logic for failed publishes
- [ ] Implement dead-letter queue for failed messages

#### 2.4 Integration with Business Logic
- [ ] Update `BookDirector` to publish messages on Create/Update/Delete
- [ ] Update `PersonDirector` to publish messages on Create/Update/Delete
- [ ] Add message publishing to Unit of Work commit
- [ ] Implement transactional publishing (optional)
- [ ] Add message correlation IDs

#### 2.5 Configuration and Dependency Injection
- [ ] Register message publisher services in DependencyInjection.cs
- [ ] Configure RabbitMQ connection settings
- [ ] Configure exchange and queue names
- [ ] Add health check for RabbitMQ connectivity
- [ ] Configure message retry policies

#### 2.6 Testing and Validation
- [ ] Test message publishing for all operations
- [ ] Verify message content and structure
- [ ] Test connection failure handling
- [ ] Test message retry logic
- [ ] Validate message ordering
- [ ] Performance testing for high-volume publishing

### Technical Specifications

#### RabbitMQ Configuration
```json
{
  "Messaging": {
    "Enabled": true,
    "RabbitMQConnectionString": "amqp://localhost:5672",
    "ExchangeName": "additional-service",
    "ExchangeType": "topic",
    "QueuePrefix": "additional-service",
    "RetryCount": 3,
    "RetryDelaySeconds": 5,
    "EnableDeadLetterQueue": true
  }
}
```

#### Message Contracts
```csharp
public class BookCreatedMessage
{
    public string BookId { get; set; }
    public BookDTO BookData { get; set; }
    public DateTime Timestamp { get; set; }
    public string CorrelationId { get; set; }
    public string OperationType => "Created";
}
```

#### Exchange/Queue Topology
- **Exchange**: `additional-service` (topic type)
- **Queues**: 
  - `additional-service.book.created`
  - `additional-service.book.updated`
  - `additional-service.book.deleted`
  - `additional-service.person.created`
  - `additional-service.person.updated`
  - `additional-service.person.deleted`
- **Routing Keys**: `book.created`, `book.updated`, etc.

#### Error Handling
- Connection retry with exponential backoff
- Dead-letter queue for failed messages
- Message acknowledgment for reliable delivery
- Logging of publishing failures

### Success Criteria
- [ ] Message publishing working for all CRUD operations
- [ ] Message content matches entity data
- [ ] Connection failure handling implemented
- [ ] Retry logic functioning correctly
- [ ] No performance degradation in API operations
- [ ] Monitoring and logging in place

### Dependencies
- Requires local/remote RabbitMQ server
- No breaking changes to existing API
- Can be deployed independently
- Optional dependency on Phase 1 (Redis)

### Estimated Effort
- **Development**: 3-4 days
- **Testing**: 2-3 days
- **Documentation**: 1 day

---

## Phase 3: Auth0 Authentication and Authorization

### Objectives
- Implement secure authentication using Auth0 OAuth2/OIDC
- Add role-based authorization for API endpoints
- Secure all API endpoints with JWT validation
- Implement user profile management

### Prerequisites
- Auth0 account and application configured
- Phases 1 and 2 completed (recommended)
- Understanding of OAuth2/OIDC flows
- SSL/TLS certificate for production

### Implementation Steps

#### 3.1 Auth0 Setup
- [ ] Create Auth0 application (API type)
- [ ] Configure Auth0 API identifier and audience
- [ ] Set up Auth0 connections (database, social providers)
- [ ] Configure Auth0 roles and permissions
- [ ] Generate Auth0 client credentials
- [ ] Configure Auth0 callback URLs

#### 3.2 Authentication Infrastructure
- [ ] Add `Microsoft.AspNetCore.Authentication.JwtBearer` NuGet package
- [ ] Add `Microsoft.AspNetCore.Authentication.OpenIdConnect` NuGet package
- [ ] Create Auth0 configuration section in appsettings.json
- [ ] Add Auth0 domain, audience, and client ID to configuration
- [ ] Configure JWT bearer authentication in Program.cs
- [ ] Implement token validation middleware

#### 3.3 Authorization Implementation
- [ ] Define authorization policies in Program.cs
- [ ] Implement role-based authorization policies
- [ ] Add permission-based authorization (optional)
- [ ] Create user context service for accessing user info
- [ ] Implement claims transformation

#### 3.4 API Endpoint Security
- [ ] Add `[Authorize]` attributes to all controllers
- [ ] Add role-based authorization to specific endpoints
- [ ] Implement endpoint-specific authorization policies
- [ ] Add authorization to health check endpoints (optional)
- [ ] Create public endpoints for authentication flow

#### 3.5 User Management (Optional)
- [ ] Create user profile endpoints
- [ ] Implement user registration endpoint
- [ ] Add user management API (if needed)
- [ ] Implement token refresh logic
- [ ] Add logout functionality

#### 3.6 Configuration and Dependency Injection
- [ ] Register authentication services in Program.cs
- [ ] Configure Auth0 settings for different environments
- [ ] Add authentication/authorization middleware
- [ ] Configure CORS for Auth0 callbacks
- [ ] Add HTTPS enforcement for production

#### 3.7 Testing and Validation
- [ ] Test authentication flow with Auth0
- [ ] Test token validation and expiration
- [ ] Test role-based authorization
- [ ] Test unauthorized access handling
- [ ] Test token refresh logic
- [ ] Security audit and penetration testing

### Technical Specifications

#### Auth0 Configuration
```json
{
  "Auth0": {
    "Domain": "your-auth0-domain.auth0.com",
    "Audience": "https://your-api-identifier",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "CallbackUrl": "https://your-api.com/callback",
    "Scope": "openid profile email"
  }
}
```

#### Authorization Policies
- **Admin**: Full access to all operations
- **User**: Read access, limited write access
- **Guest**: Read-only access to public endpoints
- **Custom**: Role-based permissions per endpoint

#### JWT Token Structure
```json
{
  "sub": "user-id",
  "name": "user-name",
  "email": "user-email",
  "roles": ["user"],
  "permissions": ["read:books", "write:books"],
  "iat": 1234567890,
  "exp": 1234567890
}
```

#### Security Considerations
- HTTPS/TLS enforcement in production
- Token expiration and refresh
- Secure storage of Auth0 credentials
- Rate limiting for authentication endpoints
- Audit logging for authentication events
- OWASP compliance

### API Versioning Strategy
Since this phase introduces breaking changes (authentication required), implement API versioning:
- **v1**: Existing API without authentication (deprecated)
- **v2**: New API with authentication required
- Provide migration period and documentation
- Implement API versioning in controllers

### Success Criteria
- [ ] Authentication flow working with Auth0
- [ ] JWT token validation functioning correctly
- [ ] Role-based authorization implemented
- [ ] All endpoints secured appropriately
- [ ] Token refresh logic working
- [ ] Security audit passed
- [ ] Performance impact minimal

### Dependencies
- Requires Auth0 account and configuration
- Breaking change: All endpoints require authentication
- Requires SSL/TLS for production
- Depends on Phases 1 and 2 (recommended)

### Estimated Effort
- **Development**: 4-5 days
- **Testing**: 3-4 days
- **Documentation**: 1-2 days
- **Security Audit**: 2-3 days

---

## Phase 4: Circuit Breaker Pattern Implementation

### Objectives
- Implement circuit breaker pattern for external service resilience
- Add retry logic with exponential backoff
- Implement fallback mechanisms for service failures
- Monitor circuit breaker state and health
- Prevent cascading failures during service outages

### Prerequisites
- Phases 0-3 completed
- Understanding of resilience patterns
- External services identified (Redis, RabbitMQ, Auth0)

### Implementation Steps

#### 4.1 Infrastructure Setup
- [ ] Add `Polly` NuGet package to API project
- [ ] Add `Microsoft.Extensions.Http.Polly` NuGet package
- [ ] Create circuit breaker configuration section in appsettings.json
- [ ] Define circuit breaker policies for each external service
- [ ] Set up circuit breaker state monitoring

#### 4.2 Circuit Breaker Policies
- [ ] Create Redis circuit breaker policy
- [ ] Create RabbitMQ circuit breaker policy
- [ ] Create Auth0 circuit breaker policy
- [ ] Implement retry policies with exponential backoff
- [ ] Configure timeout policies for each service
- [ ] Define fallback strategies for each service

#### 4.3 Service Integration
- [ ] Wrap Redis cache service with circuit breaker
- [ ] Wrap RabbitMQ message publisher with circuit breaker
- [ ] Wrap Auth0 authentication with circuit breaker
- [ ] Implement fallback logic for each service
- [ ] Add circuit breaker state logging
- [ ] Implement circuit breaker event handlers

#### 4.4 Monitoring and Health Checks
- [ ] Add circuit breaker health checks
- [ ] Implement circuit breaker state monitoring
- [ ] Create circuit breaker metrics collection
- [ ] Add circuit breaker dashboard endpoints
- [ ] Implement circuit breaker alerting
- [ ] Add circuit breaker state to health responses

#### 4.5 Configuration and Dependency Injection
- [ ] Register circuit breaker policies in DependencyInjection.cs
- [ ] Configure circuit breaker settings per service
- [ ] Add circuit breaker configuration validation
- [ ] Implement circuit breaker policy management
- [ ] Add circuit breaker configuration reload support

#### 4.6 Testing and Validation
- [ ] Test circuit breaker state transitions
- [ ] Test retry logic with exponential backoff
- [ ] Test fallback mechanisms
- [ ] Test circuit breaker recovery
- [ ] Test cascading failure prevention
- [ ] Performance testing with circuit breaker

### Technical Specifications

#### Circuit Breaker Configuration
```json
{
  "CircuitBreaker": {
    "Redis": {
      "ExceptionsAllowedBeforeBreaking": 5,
      "DurationOfBreakInSeconds": 30,
      "RetryCount": 3,
      "RetryDelayInSeconds": 1,
      "TimeoutInSeconds": 5
    },
    "RabbitMQ": {
      "ExceptionsAllowedBeforeBreaking": 3,
      "DurationOfBreakInSeconds": 60,
      "RetryCount": 5,
      "RetryDelayInSeconds": 2,
      "TimeoutInSeconds": 10
    },
    "Auth0": {
      "ExceptionsAllowedBeforeBreaking": 5,
      "DurationOfBreakInSeconds": 300,
      "RetryCount": 2,
      "RetryDelayInSeconds": 5,
      "TimeoutInSeconds": 15
    }
  }
}
```

#### Circuit Breaker Policy Example
```csharp
public static IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(r => !r.IsSuccessStatusCode)
        .CircuitBreakerAsync(
            exceptionsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30),
            onBreak: (exception, duration) => 
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
```

#### Circuit Breaker States
- **Closed**: Normal operation, requests pass through
- **Open**: Circuit is tripped, requests fail fast
- **Half-Open**: Testing if service has recovered

### Success Criteria
- [ ] Circuit breaker implemented for all external services
- [ ] Retry logic with exponential backoff working
- [ ] Fallback mechanisms functioning correctly
- [ ] Circuit breaker state monitoring operational
- [ ] No cascading failures during service outages
- [ ] Circuit breaker metrics dashboard functional
- [ ] Circuit breaker recovery working properly

### Dependencies
- Requires Polly NuGet package
- No breaking changes to existing API
- Depends on Phases 1-3 (Redis, RabbitMQ, Auth0)
- Can be developed in parallel with Phase 5

### Estimated Effort
- **Development**: 4-5 days
- **Testing**: 3-4 days
- **Documentation**: 1 day
- **Monitoring Setup**: 1-2 days

---

## Phase 5: Rate Limiting Implementation

### Objectives
- Implement API rate limiting to prevent abuse
- Ensure fair usage across all users
- Protect system resources from overload
- Support different rate limits for different user roles
- Implement distributed rate limiting using Redis

### Prerequisites
- Phase 1 (Redis) completed for distributed rate limiting
- Phase 0 (API Response Model) completed for rate limit headers
- Understanding of rate limiting algorithms

### Implementation Steps

#### 5.1 Infrastructure Setup
- [ ] Add `AspNetCoreRateLimit` NuGet package to API project
- [ ] Create rate limiting configuration section in appsettings.json
- [ ] Define rate limiting rules per user role
- [ ] Configure rate limiting algorithms (sliding window)
- [ ] Set up distributed rate limiting with Redis

#### 5.2 Rate Limiting Rules
- [ ] Define rate limits for anonymous users
- [ ] Define rate limits for authenticated users
- [ ] Define rate limits for admin users
- [ ] Configure endpoint-specific rate limits
- [ ] Implement stricter limits for write operations
- [ ] Configure rate limit periods (minute, hour, day)

#### 5.3 Rate Limiting Middleware
- [ ] Add rate limiting middleware to pipeline
- [ ] Configure rate limiting policies
- [ ] Implement IP-based rate limiting
- [ ] Implement user-based rate limiting
- [ ] Add rate limiting exception handling
- [ ] Configure rate limiting order of precedence

#### 5.4 Response Headers and Error Handling
- [ ] Add rate limit headers to responses
- [ ] Implement rate limit exceeded error responses
- [ ] Add retry-after header for exceeded limits
- [ ] Configure rate limit error format
- [ ] Add rate limit warning headers
- [ ] Implement rate limit notification system

#### 5.5 Distributed Rate Limiting
- [ ] Configure Redis for distributed rate limiting
- [ ] Implement rate limit key generation
- [ ] Add rate limit synchronization across instances
- [ ] Configure rate limit expiration in Redis
- [ ] Implement rate limit fallback when Redis unavailable

#### 5.6 Monitoring and Metrics
- [ ] Add rate limiting metrics collection
- [ ] Implement rate limit violation logging
- [ ] Create rate limiting dashboard
- [ ] Add rate limiting alerting
- [ ] Implement rate limiting analytics
- [ ] Add rate limiting to health checks

#### 5.7 Testing and Validation
- [ ] Test rate limiting per user role
- [ ] Test rate limiting per endpoint
- [ ] Test distributed rate limiting
- [ ] Test rate limit headers
- [ ] Test rate limit error responses
- [ ] Performance testing with rate limiting

### Technical Specifications

#### Rate Limiting Configuration
```json
{
  "RateLimiting": {
    "EnableRateLimiting": true,
    "UseDistributedRateLimiting": true,
    "StackExchangeRedisOptions": {
      "ConnectionMultiplexer": "localhost:6379"
    },
    "GeneralRules": {
      "Anonymous": {
        "PerMinute": 100,
        "PerHour": 1000,
        "PerDay": 10000
      },
      "Authenticated": {
        "PerMinute": 1000,
        "PerHour": 10000,
        "PerDay": 100000
      },
      "Admin": {
        "PerMinute": 5000,
        "PerHour": 50000,
        "PerDay": 500000
      }
    },
    "EndpointRules": {
      "Read": {
        "Multiplier": 1.0
      },
      "Write": {
        "Multiplier": 0.5
      },
      "Delete": {
        "Multiplier": 0.2
      }
    }
  }
}
```

#### Rate Limit Headers
```
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 950
X-RateLimit-Reset: 1640995200
X-RateLimit-Reset-After: 300
X-RateLimit-Window: 60
```

#### Rate Limiting Algorithm
- **Sliding Window Log**: Accurate but memory-intensive
- **Sliding Window Counter**: Good balance of accuracy and memory
- **Fixed Window**: Simple but can have burst issues
- **Token Bucket**: Good for rate limiting with bursts

### Success Criteria
- [ ] Rate limiting implemented for all endpoints
- [ ] Distributed rate limiting using Redis working
- [ ] Per-user and per-IP rate limiting functional
- [ ] Rate limit headers properly set
- [ ] 429 responses for exceeded limits
- [ ] Rate limiting metrics and monitoring in place
- [ ] Rate limiting performance impact < 2ms

### Dependencies
- Requires AspNetCoreRateLimit NuGet package
- Requires Redis for distributed rate limiting
- No breaking changes to existing API
- Depends on Phase 1 (Redis) and Phase 0 (Response Model)

### Estimated Effort
- **Development**: 3-4 days
- **Testing**: 2-3 days
- **Documentation**: 1 day
- **Monitoring Setup**: 1 day

---

## Implementation Order and Dependencies

### Recommended Sequence
1. **Phase 0 (API Response Model)**: Start here as foundation for all other phases
2. **Phase 1 (Redis)**: Build upon Phase 0, provides immediate performance benefits
3. **Phase 2 (RabbitMQ)**: Build upon Phase 1, adds messaging capabilities
4. **Phase 3 (Auth0)**: Implement after messaging infrastructure is ready
5. **Phase 4 (Circuit Breaker)**: Implement after external services are integrated
6. **Phase 5 (Rate Limiting)**: Implement last as it depends on Redis and response model

### Parallel Development Opportunities
- Phase 1 and Phase 2 can be developed in parallel after Phase 0
- Phase 4 and Phase 5 can be developed in parallel after Phases 1-3
- Phase 3 should be developed sequentially after Phases 1-2

### Integration Points
- All phases integrate through the Director layer
- Dependency injection configuration consolidates in DependencyInjection.cs
- Configuration management unified in appsettings.json
- Response model used across all phases
- Circuit breaker wraps external service calls
- Rate limiting applies to all API endpoints

---

## Testing Strategy

### Phase 0 Testing
- Unit tests for response models
- Integration tests for correlation tracking
- API versioning tests
- Response wrapper performance tests
- Error response validation tests
- Pagination metadata tests

### Phase 1 Testing
- Unit tests for cache service
- Integration tests for Redis connectivity
- Performance tests for cache effectiveness
- Failure scenario testing

### Phase 2 Testing
- Unit tests for message publisher
- Integration tests for RabbitMQ connectivity
- Message contract validation tests
- Failure and retry scenario testing

### Phase 3 Testing
- Authentication flow integration tests
- Authorization policy tests
- Security penetration testing
- Token refresh and expiration tests

### Phase 4 Testing
- Circuit breaker state transition tests
- Retry logic tests with exponential backoff
- Fallback mechanism tests
- Cascading failure prevention tests
- Circuit breaker recovery tests

### Phase 5 Testing
- Rate limiting per user role tests
- Rate limiting per endpoint tests
- Distributed rate limiting tests
- Rate limit header validation tests
- Rate limit error response tests
- Performance tests with rate limiting

---

## Deployment Strategy

### Development Environment
- Local Redis instance
- Local RabbitMQ instance
- Development Auth0 application
- SQLite database

### Staging Environment
- Cloud Redis (Azure Redis Cache)
- Cloud RabbitMQ (Azure Service Bus)
- Staging Auth0 application
- PostgreSQL database

### Production Environment
- Managed Redis service
- Managed RabbitMQ service
- Production Auth0 application
- PostgreSQL or SQL Server
- Load balancer and SSL/TLS

---

## Risk Mitigation

### Phase 1 Risks
- **Redis connection failures**: Implement fallback to database
- **Cache consistency issues**: Implement proper invalidation strategies
- **Performance degradation**: Monitor and optimize cache usage

### Phase 2 Risks
- **Message broker downtime**: Implement retry logic and dead-letter queues
- **Message ordering issues**: Implement proper message sequencing
- **Performance impact**: Use asynchronous publishing

### Phase 3 Risks
- **Authentication failures**: Implement proper error handling
- **Breaking changes**: Use API versioning and migration period
- **Security vulnerabilities**: Conduct security audit and penetration testing

---

## Monitoring and Observability

### Metrics to Track
- Cache hit/miss ratios (Phase 1)
- Message publishing rates and failures (Phase 2)
- Authentication success/failure rates (Phase 3)
- API response times
- Database query performance
- External service connectivity

### Logging Strategy
- Structured logging with correlation IDs
- Cache operation logging
- Message publishing logging
- Authentication event logging
- Error and exception logging

### Health Checks
- Redis connectivity health check
- RabbitMQ connectivity health check
- Auth0 connectivity health check
- Database connectivity health check
- Application health endpoint

---

## Success Metrics

### Overall Project Success
- All three phases implemented and tested
- No regression in existing functionality
- Performance improvements measurable
- Security enhancements validated
- Documentation complete
- Team trained on new features

### Phase-Specific Metrics
- **Phase 1**: 70%+ cache hit rate, 50%+ response time improvement
- **Phase 2**: 99%+ message delivery success, < 100ms publishing latency
- **Phase 3**: 100% endpoint security, < 50ms authentication overhead

---

## Timeline Estimate

### Total Project Duration
- **Phase 0**: 5-7 days
- **Phase 1**: 3-4 days
- **Phase 2**: 5-7 days
- **Phase 3**: 7-10 days
- **Phase 4**: 5-7 days
- **Phase 5**: 4-6 days
- **Integration and Testing**: 5-7 days
- **Documentation**: 3-4 days
- **Total**: 37-52 days (7-10 weeks)

### Critical Path
Phase 0 (API Response Model) and Phase 3 (Auth0) are on the critical path due to breaking changes and foundational requirements.

---

## Next Steps

1. **Begin Phase 1**: Set up Redis infrastructure and start cache implementation
2. **Create development environment**: Set up local Redis, RabbitMQ, and Auth0
3. **Define detailed specifications**: Create detailed specs for each phase
4. **Establish testing framework**: Set up automated testing infrastructure
5. **Plan deployment strategy**: Define staging and production environments

---

**This roadmap should be reviewed and updated regularly as development progresses and requirements evolve.**
