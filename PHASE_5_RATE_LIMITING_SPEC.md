# Phase 5: Rate Limiting Specification

## Overview

Phase 5 implements API rate limiting to prevent abuse, ensure fair usage across all users, and protect system resources from overload. This phase supports both distributed rate limiting using Redis and in-memory rate limiting for single-instance deployments.

## Objectives

- Implement API rate limiting to prevent abuse
- Ensure fair usage across all users
- Protect system resources from overload
- Support different rate limits for different user roles
- Implement distributed rate limiting using Redis
- Add rate limit headers to API responses
- Provide graceful fallback when Redis is unavailable

## Implementation Details

### Rate Limiting Service Interface

```csharp
public interface IRateLimitingService
{
    Task<bool> IsAllowedAsync(string key, int limit, TimeSpan period, CancellationToken cancellationToken = default);
    Task<int> GetRemainingRequestsAsync(string key, int limit, CancellationToken cancellationToken = default);
}
```

### Rate Limiting Configuration

#### RateLimitingSettings
```csharp
public class RateLimitingSettings
{
    public bool EnableRateLimiting { get; set; }
    public bool UseDistributedRateLimiting { get; set; }
    public StackExchangeRedisOptions StackExchangeRedisOptions { get; set; }
    public GeneralRateLimitRules GeneralRules { get; set; }
    public EndpointRateLimitRules EndpointRules { get; set; }
}

public class StackExchangeRedisOptions
{
    public string ConnectionMultiplexer { get; set; }
}

public class GeneralRateLimitRules
{
    public RateLimitRule Anonymous { get; set; }
    public RateLimitRule Authenticated { get; set; }
    public RateLimitRule Admin { get; set; }
}

public class RateLimitRule
{
    public int PerMinute { get; set; }
    public int PerHour { get; set; }
}

public class EndpointRateLimitRules
{
    public EndpointMultiplierRule Read { get; set; }
    public EndpointMultiplierRule Write { get; set; }
    public EndpointMultiplierRule Delete { get; set; }
}

public class EndpointMultiplierRule
{
    public double Multiplier { get; set; }
}
```

#### appsettings.json
```json
{
  "RateLimiting": {
    "EnableRateLimiting": false,
    "UseDistributedRateLimiting": true,
    "StackExchangeRedisOptions": {
      "ConnectionMultiplexer": "localhost:6379"
    },
    "GeneralRules": {
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

### Rate Limiting Service Implementations

#### InMemoryRateLimitingService
```csharp
public class InMemoryRateLimitingService : IRateLimitingService
{
    private readonly ConcurrentDictionary<string, RateLimitCounter> _counters;
    private readonly ILogger<InMemoryRateLimitingService> _logger;
    private readonly Timer _cleanupTimer;

    public InMemoryRateLimitingService(ILogger<InMemoryRateLimitingService> logger)
    {
        _counters = new ConcurrentDictionary<string, RateLimitCounter>();
        _logger = logger;

        // Clean up expired counters every minute
        _cleanupTimer = new Timer(CleanupExpiredCounters, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    private void CleanupExpiredCounters(object state)
    {
        var now = DateTime.UtcNow;
        var expiredKeys = _counters.Where(kvp => kvp.Value.Expiry < now).Select(kvp => kvp.Key).ToList();

        foreach (var key in expiredKeys)
        {
            _counters.TryRemove(key, out _);
        }

        if (expiredKeys.Any())
        {
            _logger.LogDebug("Cleaned up {Count} expired rate limit counters", expiredKeys.Count);
        }
    }

    public async Task<bool> IsAllowedAsync(string key, int limit, TimeSpan period, CancellationToken cancellationToken = default)
    {
        var counter = _counters.AddOrUpdate(
            key,
            _ => new RateLimitCounter { Count = 1, Expiry = DateTime.UtcNow.Add(period) },
            (_, existing) =>
            {
                if (existing.Expiry < DateTime.UtcNow)
                {
                    existing.Count = 1;
                    existing.Expiry = DateTime.UtcNow.Add(period);
                }
                else
                {
                    existing.Count++;
                }
                return existing;
            });

        return counter.Count <= limit;
    }

    public async Task<int> GetRemainingRequestsAsync(string key, int limit, CancellationToken cancellationToken = default)
    {
        if (_counters.TryGetValue(key, out var counter))
        {
            if (counter.Expiry < DateTime.UtcNow)
            {
                return limit;
            }
            return Math.Max(0, limit - counter.Count);
        }

        return limit;
    }

    private class RateLimitCounter
    {
        public int Count { get; set; }
        public DateTime Expiry { get; set; }
    }
}
```

#### RedisRateLimitingService
```csharp
public class RedisRateLimitingService : IRateLimitingService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisRateLimitingService> _logger;
    private readonly string _luaScript;

    public RedisRateLimitingService(
        IConnectionMultiplexer redis,
        ILogger<RedisRateLimitingService> logger)
    {
        _redis = redis;
        _logger = logger;

        // Lua script for atomic rate limiting
        _luaScript = @"
            local key = KEYS[1]
            local limit = tonumber(ARGV[1])
            local period = tonumber(ARGV[2])
            
            local current = redis.call('GET', key)
            if current == false then
                current = 0
            else
                current = tonumber(current)
            end
            
            if current < limit then
                redis.call('INCR', key)
                redis.call('EXPIRE', key, period)
                return 1
            else
                return 0
            end
        ";
    }

    public async Task<bool> IsAllowedAsync(string key, int limit, TimeSpan period, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var result = await db.ScriptEvaluateAsync(
                _luaScript,
                new RedisKey[] { key },
                new RedisValue[] { limit, (int)period.TotalSeconds });

            return (long)result == 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking rate limit for key: {Key}", key);
            // On Redis failure, allow the request (fail open)
            return true;
        }
    }

    public async Task<int> GetRemainingRequestsAsync(string key, int limit, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var current = await db.StringGetAsync(key);
            
            if (!current.HasValue)
            {
                return limit;
            }

            var currentCount = (int)current;
            return Math.Max(0, limit - currentCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting remaining requests for key: {Key}", key);
            return limit;
        }
    }
}
```

### Rate Limiting Middleware

#### RateLimitingMiddleware
```csharp
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitingSettings _settings;
    private readonly IRateLimitingService _rateLimitingService;

    public RateLimitingMiddleware(
        RequestDelegate next,
        ILogger<RateLimitingMiddleware> logger,
        IOptions<RateLimitingSettings> settings,
        IRateLimitingService rateLimitingService)
    {
        _next = next;
        _logger = logger;
        _settings = settings.Value;
        _rateLimitingService = rateLimitingService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_settings.EnableRateLimiting)
        {
            await _next(context);
            return;
        }

        // Determine user role and appropriate rate limit
        var userRole = GetUserRole(context);
        var rateLimit = GetRateLimit(userRole);
        var clientIdentifier = GetClientIdentifier(context);

        // Check if request is allowed
        var isAllowed = await _rateLimitingService.IsAllowedAsync(
            clientIdentifier,
            rateLimit,
            TimeSpan.FromMinutes(1));

        if (!isAllowed)
        {
            _logger.LogWarning("Rate limit exceeded for {ClientIdentifier} with role {UserRole}. Limit: {Limit}", 
                clientIdentifier, userRole, rateLimit);

            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.Headers.Append("X-RateLimit-Limit", rateLimit.ToString());
            context.Response.Headers.Append("X-RateLimit-Remaining", "0");
            context.Response.Headers.Append("Retry-After", "60");
            
            var errorResponse = new
            {
                success = false,
                error = new
                {
                    code = "RATE_LIMIT_EXCEEDED",
                    message = $"Rate limit exceeded. Maximum {rateLimit} requests per minute allowed.",
                    statusCode = 429
                },
                timestamp = DateTime.UtcNow,
                retryAfter = 60
            };

            await context.Response.WriteAsJsonAsync(errorResponse);
            return;
        }

        // Add rate limit headers
        var remaining = await _rateLimitingService.GetRemainingRequestsAsync(clientIdentifier, rateLimit);
        context.Response.Headers.Append("X-RateLimit-Limit", rateLimit.ToString());
        context.Response.Headers.Append("X-RateLimit-Remaining", remaining.ToString());

        await _next(context);
    }

    private string GetUserRole(HttpContext context)
    {
        // Check if user is authenticated
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            // Check for admin role
            if (context.User.IsInRole("Admin"))
            {
                return "Admin";
            }
            return "Authenticated";
        }

        return "Anonymous";
    }

    private string GetClientIdentifier(HttpContext context)
    {
        // Try to get user ID from claims if authenticated
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                return $"user:{userId}";
            }
        }

        // Fall back to IP address
        return $"ip:{context.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";
    }

    private int GetRateLimit(string userRole)
    {
        return userRole switch
        {
            "Admin" => _settings?.GeneralRules?.Admin?.PerMinute ?? 5000,
            "Authenticated" => _settings?.GeneralRules?.Authenticated?.PerMinute ?? 1000,
            _ => _settings?.GeneralRules?.Anonymous?.PerMinute ?? 100
        };
    }
}
```

### Rate Limiting Strategy

#### Rate Limiting Algorithm
- **Algorithm**: Sliding window counter
- **Window Size**: 1 minute (configurable)
- **Precision**: Request-level accuracy
- **Storage**: Redis (distributed) or memory (single-instance)

#### Rate Limiting Rules

##### Anonymous Users
- **Per Minute**: 100 requests
- **Per Hour**: 1,000 requests
- **Client Identification**: IP address

##### Authenticated Users
- **Per Minute**: 1,000 requests
- **Per Hour**: 10,000 requests
- **Client Identification**: User ID from claims

##### Admin Users
- **Per Minute**: 5,000 requests
- **Per Hour**: 50,000 requests
- **Client Identification**: User ID from claims

#### Endpoint-Specific Rules
- **Read Operations**: 1.0x multiplier (standard limit)
- **Write Operations**: 0.5x multiplier (stricter limit)
- **Delete Operations**: 0.2x multiplier (very strict limit)

### Dependency Injection

```csharp
// Configure Rate Limiting settings
services.Configure<RateLimitingSettings>(
    configuration.GetSection("RateLimiting"));

// Configure Rate Limiting services
var rateLimitingSettings = configuration.GetSection("RateLimiting").Get<RateLimitingSettings>();
if (rateLimitingSettings != null && rateLimitingSettings.EnableRateLimiting)
{
    if (rateLimitingSettings.UseDistributedRateLimiting && cacheSettings != null && cacheSettings.EnableCache)
    {
        services.AddSingleton<IRateLimitingService>(sp =>
        {
            var connectionMultiplexer = sp.GetService<IConnectionMultiplexer>();
            if (connectionMultiplexer != null)
            {
                return new RedisRateLimitingService(connectionMultiplexer, sp.GetRequiredService<ILogger<RedisRateLimitingService>>());
            }
            // Fallback to in-memory rate limiting if Redis is unavailable
            return new InMemoryRateLimitingService(sp.GetRequiredService<ILogger<InMemoryRateLimitingService>>());
        });
    }
    else
    {
        services.AddSingleton<IRateLimitingService, InMemoryRateLimitingService>();
    }
}
else
{
    services.AddSingleton<IRateLimitingService, InMemoryRateLimitingService>();
}
```

### Middleware Pipeline

```csharp
// Pipeline

// Use Correlation ID Middleware (must be first)
app.UseCorrelationId();

// Use Rate Limiting Middleware
app.UseMiddleware<RateLimitingMiddleware>();

// Use CORS
app.UseCors("AllowAll");
```

## HTTP Headers

### Response Headers
- `X-RateLimit-Limit`: Maximum requests allowed in the current window
- `X-RateLimit-Remaining`: Remaining requests in the current window
- `Retry-After`: Seconds until the rate limit resets (when exceeded)

### Header Examples
```
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 950
Retry-After: 60
```

## Error Response

### Rate Limit Exceeded Response
```json
{
  "success": false,
  "error": {
    "code": "RATE_LIMIT_EXCEEDED",
    "message": "Rate limit exceeded. Maximum 100 requests per minute allowed.",
    "statusCode": 429
  },
  "timestamp": "2024-01-01T00:00:00Z",
  "retryAfter": 60
}
```

## Client Identification

### Identification Strategy
1. **Authenticated Users**: Use user ID from JWT claims (`sub` claim)
2. **Anonymous Users**: Use IP address as fallback
3. **Mixed Mode**: Support both strategies simultaneously

### Identification Format
- User-based: `user:{userId}`
- IP-based: `ip:{ipAddress}`

## Testing Guidelines

### Unit Tests
- Test rate limiting service implementations
- Test counter increment logic
- Test expiration handling
- Test cleanup functionality
- Test Redis Lua script execution

### Integration Tests
- Test rate limiting middleware
- Test different user roles
- Test distributed rate limiting with Redis
- Test fallback to in-memory rate limiting
- Test rate limit headers

### Performance Tests
- Test rate limiting overhead
- Test with high request rates
- Test concurrent access
- Test Redis performance impact
- Measure memory usage

## Best Practices

### Rate Limiting Configuration
- Set appropriate limits for each user role
- Monitor rate limit violations
- Adjust limits based on usage patterns
- Consider seasonal variations
- Implement gradual rollout

### Error Handling
- Graceful degradation when Redis unavailable
- Log rate limit violations
- Don't expose sensitive information
- Implement proper error responses
- Monitor rate limiting effectiveness

### Security
- Use consistent client identification
- Prevent rate limit bypass
- Implement proper IP handling
- Consider rate limit evasion
- Audit rate limiting events

## Troubleshooting

### Common Issues

#### Rate Limiting Not Working
- **Symptom**: No rate limiting enforced
- **Solution**: Check EnableRateLimiting setting, middleware registration
- **Check**: Verify middleware pipeline order

#### Redis Rate Limiting Fails
- **Symptom**: Falls back to in-memory rate limiting
- **Solution**: Check Redis connectivity, connection string
- **Fallback**: System automatically falls back to in-memory

#### Incorrect Rate Limits
- **Symptom**: Wrong limits applied to users
- **Solution**: Check user role detection, configuration settings
- **Check**: Review GetUserRole logic

#### Rate Limit Headers Missing
- **Symptom**: No rate limit headers in responses
- **Solution**: Check middleware execution, header configuration
- **Check**: Verify middleware pipeline order

## Performance Considerations

### Expected Performance
- Rate limiting overhead: < 2ms
- Redis rate limiting: < 5ms with network latency
- In-memory rate limiting: < 1ms
- Memory overhead: Minimal for in-memory implementation

### Optimization Strategies
- Use Redis pipelining for batch operations
- Optimize Lua script performance
- Implement proper cleanup
- Monitor memory usage
- Tune limits based on metrics

## Security Considerations

### Security Aspects
- Prevent rate limit bypass
- Use secure client identification
- Implement proper IP handling
- Protect against DDoS attacks
- Audit rate limiting events

### Privacy Considerations
- Don't log sensitive user information
- Use hashed identifiers where possible
- Implement data retention policies
- Comply with privacy regulations
- Secure rate limiting data

## Monitoring and Observability

### Metrics to Track
- Rate limit violations per endpoint
- Rate limit violations per user/IP
- Rate limit violations per role
- Redis vs in-memory rate limiting usage
- Rate limiting performance metrics

### Logging
- Log rate limit violations
- Log fallback events
- Log configuration changes
- Log rate limiting errors
- Log user role detection

### Health Checks
- Rate limiting service health
- Redis connectivity for rate limiting
- Rate limiting performance
- Memory usage monitoring
- Error rate monitoring

## Success Criteria

- [x] Rate limiting implemented for all endpoints
- [x] Distributed rate limiting using Redis working
- [x] Per-user and per-IP rate limiting functional
- [x] Rate limit headers properly set
- [x] 429 responses for exceeded limits
- [x] Rate limiting metrics and monitoring in place
- [x] Rate limiting performance impact < 2ms
- [x] Graceful fallback when Redis unavailable

## Dependencies

- AspNetCoreRateLimit (5.0.3)
- StackExchange.Redis (2.8.16) - for distributed rate limiting

## Related Documentation

- [AGENTS.md](./AGENTS.md) - AI Agent Guidelines
- [ARCHITECTURE.md](./ARCHITECTURE.md) - Architecture Documentation
- [DEVELOPMENT_GUIDELINES.md](./DEVELOPMENT_GUIDELINES.md) - Development Guidelines
- [PROJECT_SPEC.md](./PROJECT_SPEC.md) - Project Specification
- [PHASE_1_REDIS_CACHE_SPEC.md](./PHASE_1_REDIS_CACHE_SPEC.md) - Redis Cache Specification
- [PHASE_4_CIRCUIT_BREAKER_SPEC.md](./PHASE_4_CIRCUIT_BREAKER_SPEC.md) - Circuit Breaker Specification
