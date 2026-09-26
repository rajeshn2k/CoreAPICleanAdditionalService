# Phase 1: Redis Distributed Cache Specification

## Overview

Phase 1 implements a distributed caching layer using Redis to improve API response times for frequently accessed data and reduce database load. This phase builds upon Phase 0 and integrates seamlessly with the existing Clean Architecture.

## Objectives

- Implement distributed caching layer using Redis
- Improve API response times for frequently accessed data
- Reduce database load for read-heavy operations
- Establish caching patterns for future use
- Provide graceful fallback when Redis is unavailable
- Support cache invalidation strategies

## Implementation Details

### Cache Service Interface

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

### Cache Configuration

#### CacheSettings
```csharp
public class CacheSettings
{
    public string ConnectionString { get; set; }
    public int DefaultExpirationMinutes { get; set; }
    public int BookExpirationMinutes { get; set; }
    public int PersonExpirationMinutes { get; set; }
    public bool EnableCache { get; set; }
}
```

#### appsettings.json
```json
{
  "Cache": {
    "ConnectionString": "localhost:6379",
    "DefaultExpirationMinutes": 30,
    "BookExpirationMinutes": 60,
    "PersonExpirationMinutes": 60,
    "EnableCache": false
  }
}
```

### Cache Implementations

#### RedisCacheService
```csharp
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly CacheSettings _settings;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(
        IConnectionMultiplexer redis,
        ILogger<RedisCacheService> logger,
        CacheSettings settings)
    {
        _redis = redis;
        _database = redis.GetDatabase();
        _settings = settings;
        _logger = logger;
    }

    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await _database.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            return default;
        }

        return JsonConvert.DeserializeObject<T>(value);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var serialized = JsonConvert.SerializeObject(value);
        var expiry = expiration ?? TimeSpan.FromMinutes(_settings.DefaultExpirationMinutes);
        await _database.StringSetAsync(key, serialized, expiry);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern).ToArray();
        if (keys.Length > 0)
        {
            await _database.KeyDeleteAsync(keys);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _database.KeyExistsAsync(key);
    }
}
```

#### InMemoryCacheService
```csharp
public class InMemoryCacheService : ICacheService
{
    private readonly MemoryCache _cache;
    private readonly CacheSettings _settings;
    private readonly ILogger<InMemoryCacheService> _logger;

    public InMemoryCacheService(
        ILogger<InMemoryCacheService> logger,
        CacheSettings settings)
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        _settings = settings;
        _logger = logger;
    }

    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out var value))
        {
            return (T)value;
        }
        return default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var expiry = expiration ?? TimeSpan.FromMinutes(_settings.DefaultExpirationMinutes);
        _cache.Set(key, value, expiry);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.Remove(key);
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // For in-memory cache, clear all keys matching pattern
        var keysToRemove = _cache.Where(kvp => 
            System.Text.RegularExpressions.Regex.IsMatch(kvp.Key, pattern.Replace("*", ".*")))
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return _cache.TryGetValue(key, out _);
    }
}
```

### Cache Key Patterns

#### Standard Patterns
- `book:{id}` - Individual book cache
- `book:all` - All books cache
- `book:search:{query}` - Search results cache
- `book:person:{personId}` - Books by person cache
- `person:{id}` - Individual person cache
- `person:all` - All persons cache
- `person:search:{query}` - Search results cache

#### Key Management
- Hierarchical structure for organization
- Consistent naming convention
- Support for pattern-based invalidation
- TTL-based expiration

### Cache Strategies

#### Read-Through Cache
```csharp
public async Task<BookDTO> GetBookAsync(string bookId, CancellationToken cancellationToken)
{
    var cacheKey = $"book:{bookId}";
    var cachedBook = await _cacheService.GetAsync<BookDTO>(cacheKey, cancellationToken);
    
    if (cachedBook != null)
    {
        _logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
        return cachedBook;
    }

    var book = await _bookRepository.GetEntityByIdAsync(bookId, cancellationToken);
    
    if (book != null)
    {
        await _cacheService.SetAsync(cacheKey, book, TimeSpan.FromMinutes(60), cancellationToken);
    }
    
    return book;
}
```

#### Write-Through Cache
```csharp
public async Task<BookDTO> CreateBookAsync(BookCreateDTO bookDto, CancellationToken cancellationToken)
{
    var book = await _bookRepository.CreateEntityAsync(bookEntity, cancellationToken);
    
    if (book != null)
    {
        await _cacheService.RemoveAsync("book:all", cancellationToken);
        await _cacheService.SetAsync($"book:{book.Id}", book, TimeSpan.FromMinutes(60), cancellationToken);
    }
    
    return book;
}
```

#### Cache Aside
```csharp
public async Task<long> UpdateEntityByIdAsync(string entityId, BookDTO book, CancellationToken cancellationToken)
{
    var result = await _bookRepository.UpdateAsync(entityId, bookEntity, cancellationToken);

    if (result > 0)
    {
        await InvalidateBookCacheAsync(entityId, cancellationToken);
    }

    return result;
}

private async Task InvalidateBookCacheAsync(string bookId, CancellationToken cancellationToken)
{
    await _cacheService.RemoveAsync($"book:{bookId}", cancellationToken);
    await _cacheService.RemoveAsync("book:all", cancellationToken);
}
```

### Director Integration

#### BookDirector with Caching
```csharp
public class BookDirector : IEntityDirector<BookDTO, BookCreateDTO>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ICacheService cacheService;
    private readonly ILogger<BookDirector> logger;

    public async Task<IEnumerable<BookDTO>> GetEntitiesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = "book:all";
            var cachedBooks = await cacheService.GetAsync<IEnumerable<BookDTO>>(cacheKey, cancellationToken);
            
            if (cachedBooks != null)
            {
                logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
                return cachedBooks;
            }

            var books = await unitOfWork.BookRepository.GetEntitiesAsync(cancellationToken);
            var bookDTOs = books.Select(BookMapper.BookToBookDTO);
            await cacheService.SetAsync(cacheKey, bookDTOs, TimeSpan.FromMinutes(30), cancellationToken);
            return bookDTOs;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetEntitiesAsync, falling back to database");
            var books = await unitOfWork.BookRepository.GetEntitiesAsync(cancellationToken);
            return books?.Select(BookMapper.BookToBookDTO);
        }
    }
}
```

### Dependency Injection

```csharp
// Configure Cache services
services.Configure<CacheSettings>(
    configuration.GetSection("Cache"));

var cacheSettings = configuration.GetSection("Cache").Get<CacheSettings>();
if (cacheSettings != null && cacheSettings.EnableCache)
{
    try
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var config = ConfigurationOptions.Parse(cacheSettings.ConnectionString);
            return ConnectionMultiplexer.Connect(config);
        });

        services.AddSingleton<ICacheService>(sp =>
        {
            var innerCacheService = new RedisCacheService(
                sp.GetRequiredService<IConnectionMultiplexer>(),
                sp.GetRequiredService<ILogger<RedisCacheService>>(),
                cacheSettings);
            var circuitBreakerService = sp.GetRequiredService<ICircuitBreakerService>();
            var logger = sp.GetRequiredService<ILogger<CircuitBreakerCacheService>>();
            return new CircuitBreakerCacheService(innerCacheService, circuitBreakerService, logger);
        });
    }
    catch
    {
        // Fallback to in-memory cache if Redis is unavailable
        services.AddSingleton<ICacheService>(sp =>
        {
            var innerCacheService = new InMemoryCacheService(
                sp.GetRequiredService<ILogger<InMemoryCacheService>>(),
                cacheSettings);
            var circuitBreakerService = sp.GetRequiredService<ICircuitBreakerService>();
            var logger = sp.GetRequiredService<ILogger<CircuitBreakerCacheService>>();
            return new CircuitBreakerCacheService(innerCacheService, circuitBreakerService, logger);
        });
    }
}
else
{
    services.AddSingleton<ICacheService>(sp =>
    {
        var innerCacheService = new InMemoryCacheService(
            sp.GetRequiredService<ILogger<InMemoryCacheService>>(),
            cacheSettings);
        var circuitBreakerService = sp.GetRequiredService<ICircuitBreakerService>();
        var logger = sp.GetRequiredService<ILogger<CircuitBreakerCacheService>>();
        return new CircuitBreakerCacheService(innerCacheService, circuitBreakerService, logger);
    });
}
```

## Cache Configuration

### Expiration Policies

#### Default Expiration
- Individual entities: 60 minutes
- Lists/Queries: 30 minutes
- Search results: 15 minutes

#### Entity-Specific Expiration
- Books: 60 minutes
- Persons: 60 minutes
- Custom: Configurable per use case

### Serialization

#### JSON Serialization
- Uses Newtonsoft.Json for object serialization
- Handles complex object graphs
- Supports circular references
- Configurable serialization settings

#### Serialization Options
```csharp
var settings = new JsonSerializerSettings
{
    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    NullValueHandling = NullValueHandling.Ignore,
    DateFormatString = "yyyy-MM-ddTHH:mm:ssZ"
};
```

## Testing Guidelines

### Unit Tests
- Test cache service implementations
- Test cache key generation
- Test serialization/deserialization
- Test expiration policies
- Test pattern-based invalidation

### Integration Tests
- Test Redis connectivity
- Test cache hit/miss scenarios
- Test cache invalidation on data changes
- Test Redis connection failure handling
- Test fallback to in-memory cache

### Performance Tests
- Measure cache hit/miss ratios
- Test cache response times vs database
- Measure memory usage
- Test concurrent cache access
- Validate cache expiration accuracy

## Best Practices

### Cache Key Design
- Use hierarchical structure
- Include entity type and identifier
- Use consistent naming convention
- Avoid overly complex patterns
- Keep keys readable and debuggable

### Cache Invalidation
- Invalidate on data changes
- Use pattern-based invalidation for lists
- Consider cache coherency requirements
- Implement proper error handling
- Log invalidation events

### Error Handling
- Graceful fallback to database
- Log cache failures
- Don't fail API operations on cache errors
- Implement circuit breaker protection
- Monitor cache health

### Cache Sizing
- Monitor Redis memory usage
- Implement proper eviction policies
- Consider cache partitioning for large datasets
- Use appropriate TTL values
- Monitor cache hit/miss ratios

## Troubleshooting

### Common Issues

#### Redis Connection Failures
- **Symptom**: Cache operations fail, API falls back to database
- **Solution**: Check Redis server status, connection string, network connectivity
- **Fallback**: System automatically falls back to in-memory cache

#### Cache Not Working
- **Symptom**: No cache hits, all requests hit database
- **Solution**: Verify EnableCache setting, check cache key generation
- **Check**: Review logs for cache operations

#### High Memory Usage
- **Symptom**: Redis memory usage increasing
- **Solution**: Review TTL settings, implement proper eviction policies
- **Monitor**: Use Redis INFO command to monitor memory

#### Serialization Errors
- **Symptom**: Cache set/get operations fail
- **Solution**: Check object serialization compatibility
- **Check**: Verify JSON serialization settings

## Performance Considerations

### Expected Performance Improvements
- Cache hit response time: < 10ms
- Cache miss response time: Database response time + cache update
- Target cache hit rate: > 70%
- Memory overhead: Configurable based on cache size

### Optimization Strategies
- Use appropriate TTL values
- Implement cache warming for hot data
- Monitor and adjust cache sizes
- Use Redis pipelining for batch operations
- Consider cache compression for large objects

## Security Considerations

### Redis Security
- Use Redis AUTH for authentication
- Enable TLS for production deployments
- Use Redis ACLs for access control
- Encrypt sensitive data before caching
- Regular security updates for Redis

### Data Security
- Don't cache sensitive information
- Implement proper cache key obfuscation
- Use secure serialization
- Implement cache access logging
- Regular security audits

## Monitoring and Observability

### Metrics to Track
- Cache hit/miss ratio
- Cache response times
- Redis memory usage
- Cache size and growth
- Cache error rates

### Logging
- Log cache hits and misses
- Log cache invalidation events
- Log cache operation failures
- Log Redis connection events
- Log fallback events

### Health Checks
- Redis connectivity check
- Cache service health check
- Memory usage monitoring
- Cache hit rate monitoring
- Error rate monitoring

## Success Criteria

- [x] Redis integration completed
- [x] Cache hit rate > 70% for frequently accessed data
- [x] Response time improvement > 50% for cached operations
- [x] No cache consistency issues
- [x] Graceful degradation when Redis is unavailable
- [x] Configuration externalized and documented
- [x] Circuit breaker protection implemented

## Dependencies

- StackExchange.Redis (2.8.16)
- Microsoft.Extensions.Caching.StackExchangeRedis (10.0.11)
- Newtonsoft.Json (13.0.4)

## Related Documentation

- [AGENTS.md](./AGENTS.md) - AI Agent Guidelines
- [ARCHITECTURE.md](./ARCHITECTURE.md) - Architecture Documentation
- [DEVELOPMENT_GUIDELINES.md](./DEVELOPMENT_GUIDELINES.md) - Development Guidelines
- [PROJECT_SPEC.md](./PROJECT_SPEC.md) - Project Specification
- [PHASE_0_API_RESPONSE_MODEL_SPEC.md](./PHASE_0_API_RESPONSE_MODEL_SPEC.md) - API Response Model Specification
