# Phase 0: API Response Model Specification

## Overview

Phase 0 implements a standardized API response model across all endpoints to provide consistent error handling, correlation tracking, and improved debugging capabilities. This phase serves as the foundation for all subsequent phases.

## Objectives

- Implement consistent API response format across all endpoints
- Add proper error handling and standardized error responses
- Implement request/response correlation tracking
- Add pagination metadata for list responses
- Establish foundation for future monitoring and debugging
- Support API versioning for breaking changes

## Implementation Details

### Response Models

#### ApiResponse<T>
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
```

#### ApiErrorResponse
```csharp
public class ApiErrorResponse
{
    public bool Success { get; set; }
    public ErrorDetail Error { get; set; }
    public DateTime Timestamp { get; set; }
    public string RequestId { get; set; }
    public string Path { get; set; }
}
```

#### PaginationMetadata
```csharp
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

### Error Codes

```csharp
public static class ErrorCodes
{
    public const string VALIDATION_ERROR = "VALIDATION_ERROR";
    public const string NOT_FOUND = "NOT_FOUND";
    public const string BOOK_NOT_FOUND = "BOOK_NOT_FOUND";
    public const string PERSON_NOT_FOUND = "PERSON_NOT_FOUND";
    public const string UNAUTHORIZED = "UNAUTHORIZED";
    public const string FORBIDDEN = "FORBIDDEN";
    public const string INTERNAL_ERROR = "INTERNAL_ERROR";
    public const string RATE_LIMIT_EXCEEDED = "RATE_LIMIT_EXCEEDED";
    public const string SERVICE_UNAVAILABLE = "SERVICE_UNAVAILABLE";
}
```

### Correlation ID Middleware

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

### Controller Examples

#### Success Response
```csharp
[HttpGet]
public async Task<ActionResult<ApiResponse<IEnumerable<BookDTO>>>> Get()
{
    var correlationId = HttpContext.GetCorrelationId();
    var books = await _bookDirector.GetEntitiesAsync(default);

    var response = ApiResponse<IEnumerable<BookDTO>>.CreateSuccess(
        books,
        "Books retrieved successfully",
        correlationId);

    return Ok(response);
}
```

#### Error Response
```csharp
[HttpGet("{bookId}")]
public async Task<ActionResult<ApiResponse<BookDTO>>> GetById(string bookId)
{
    var correlationId = HttpContext.GetCorrelationId();
    var book = await _bookDirector.GetEntityByIdAsync(bookId, default);

    if (book == null)
    {
        var errorResponse = ApiErrorResponse.CreateError(
            ErrorCodes.BOOK_NOT_FOUND,
            $"Book with ID {bookId} not found",
            404,
            correlationId,
            HttpContext.Request.Path);

        return NotFound(errorResponse);
    }

    var response = ApiResponse<BookDTO>.CreateSuccess(
        book,
        "Book retrieved successfully",
        correlationId);

    return Ok(response);
}
```

## Configuration

### appsettings.json
```json
{
  "ApiResponse": {
    "IncludeTimestamp": true,
    "IncludeRequestId": true,
    "DetailedErrors": true
  }
}
```

### ApiResponseSettings
```csharp
public class ApiResponseSettings
{
    public bool IncludeTimestamp { get; set; }
    public bool IncludeRequestId { get; set; }
    public bool DetailedErrors { get; set; }
}
```

## API Versioning

### Versioning Strategy
- **v1**: Legacy API without standardized response format (deprecated)
- **v2**: Current API with standardized response format
- Default version: v2.0
- Version detection via URL segment, header, or query string

### Version Configuration
```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(2, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"),
        new QueryStringApiVersionReader("api-version"));
});
```

## Response Examples

### Success Response
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
  "requestId": "correlation-id-guid"
}
```

### Error Response
```json
{
  "success": false,
  "error": {
    "code": "BOOK_NOT_FOUND",
    "message": "Book with ID 123 not found",
    "statusCode": 404
  },
  "timestamp": "2024-01-01T00:00:00Z",
  "requestId": "correlation-id-guid",
  "path": "/api/v2/Book/123"
}
```

### Paginated Response
```json
{
  "success": true,
  "data": [...],
  "message": "Books retrieved successfully",
  "timestamp": "2024-01-01T00:00:00Z",
  "requestId": "correlation-id-guid",
  "pagination": {
    "currentPage": 1,
    "pageSize": 10,
    "totalItems": 100,
    "totalPages": 10,
    "hasPrevious": false,
    "hasNext": true
  }
}
```

## HTTP Headers

### Request Headers
- `X-Correlation-ID`: Optional correlation ID for request tracking

### Response Headers
- `X-Correlation-ID`: Correlation ID for request/response tracking
- `X-API-Version`: API version used for the request

## Testing Guidelines

### Unit Tests
- Test response model creation and validation
- Test error code constants
- Test correlation ID generation and propagation
- Test pagination metadata calculation

### Integration Tests
- Test all endpoints return standardized responses
- Verify correlation ID is consistent across request/response
- Test error responses for different error scenarios
- Verify API versioning works correctly

### Performance Tests
- Measure response wrapping overhead (< 5ms target)
- Test correlation ID middleware performance impact
- Validate API versioning doesn't add significant latency

## Best Practices

### Response Creation
- Always use `ApiResponse<T>.CreateSuccess()` for success responses
- Always use `ApiErrorResponse.CreateError()` for error responses
- Include meaningful messages for all responses
- Use appropriate HTTP status codes

### Error Handling
- Catch exceptions at controller level
- Return standardized error responses
- Log errors with correlation ID
- Don't expose sensitive information in error messages

### Correlation Tracking
- Include correlation ID in all log entries
- Pass correlation ID to external service calls
- Use correlation ID for distributed tracing
- Maintain correlation ID across async operations

## Troubleshooting

### Common Issues

#### Missing Correlation ID
- **Symptom**: Correlation ID not present in response headers
- **Solution**: Ensure CorrelationIdMiddleware is registered first in the pipeline
- **Check**: Verify middleware order in Program.cs

#### Response Not Wrapped
- **Symptom**: API returns raw data instead of ApiResponse<T>
- **Solution**: Ensure controllers use ApiResponse<T> wrapper
- **Check**: Verify V2 controllers are being used (not V1)

#### API Version Not Detected
- **Symptom**: Default version not applied correctly
- **Solution**: Check API versioning configuration
- **Check**: Verify version reader configuration includes URL segment reader

## Migration Guide

### For API Consumers

#### Breaking Changes
- Response structure changed from raw data to wrapped format
- Error response format changed
- API versioning requires version specification

#### Migration Steps
1. Update API client to handle ApiResponse<T> wrapper
2. Update error handling to use ApiErrorResponse format
3. Add API version to requests (header, query, or URL)
4. Update correlation ID handling for debugging
5. Test all endpoints with new response format

#### Example Migration
```csharp
// Old response handling
var book = await response.Content.ReadFromJsonAsync<BookDTO>();

// New response handling
var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<BookDTO>>();
var book = apiResponse.Data;
```

## Dependencies

- Microsoft.AspNetCore.Mvc.Versioning (5.1.0)
- Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer (5.1.0)
- No external dependencies required

## Performance Considerations

- Response wrapping overhead: < 5ms target
- Correlation ID generation: negligible overhead
- API versioning: minimal performance impact
- Memory usage: Minimal increase due to wrapper objects

## Security Considerations

- Correlation IDs don't contain sensitive information
- Error messages don't expose internal implementation details
- API versioning doesn't introduce security vulnerabilities
- Response structure validation prevents injection attacks

## Monitoring and Observability

### Metrics to Track
- Response wrapper creation time
- Correlation ID generation success rate
- API version usage distribution
- Error response rates by error code

### Logging
- Log correlation ID for all requests
- Log response creation failures
- Log API version mismatches
- Log error response details

## Success Criteria

- [x] All endpoints using standardized response format
- [x] Correlation ID tracking working across all requests
- [x] Error responses consistent and informative
- [x] API versioning functional
- [x] Response wrapping overhead < 5ms
- [x] Migration guide completed

## Related Documentation

- [AGENTS.md](./AGENTS.md) - AI Agent Guidelines
- [ARCHITECTURE.md](./ARCHITECTURE.md) - Architecture Documentation
- [DEVELOPMENT_GUIDELINES.md](./DEVELOPMENT_GUIDELINES.md) - Development Guidelines
- [PROJECT_SPEC.md](./PROJECT_SPEC.md) - Project Specification
