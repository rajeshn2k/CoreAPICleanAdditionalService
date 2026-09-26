# Phase 4: Circuit Breaker Pattern Specification

## Overview

Phase 4 implements the circuit breaker pattern for external service resilience and fault tolerance. This phase protects the API from cascading failures when external services (Redis, RabbitMQ) are unavailable or experiencing issues.

## Objectives

- Implement circuit breaker pattern for external service resilience
- Add retry logic with exponential backoff
- Implement fallback mechanisms for service failures
- Monitor circuit breaker state and health
- Prevent cascading failures during service outages
- Provide graceful degradation when services are unavailable

## Implementation Details

### Circuit Breaker Service Interface

```csharp
public interface ICircuitBreakerService
{
    Task<T> ExecuteAsync<T>(string serviceKey, Func<Task<T>> action, CancellationToken cancellationToken = default);
    Task ExecuteAsync(string serviceKey, Func<Task> action, CancellationToken cancellationToken = default);
    CircuitBreakerState GetState(string serviceKey);
}
```

### Circuit Breaker States

```csharp
public enum CircuitBreakerState
{
    Closed,    // Normal operation, requests pass through
    Open,       // Circuit is tripped, requests fail fast
    HalfOpen    // Testing if service has recovered
}
```

### Circuit Breaker Configuration

#### CircuitBreakerSettings
```csharp
public class CircuitBreakerSettings
{
    public RedisCircuitBreakerSettings Redis { get; set; }
    public RabbitMQCircuitBreakerSettings RabbitMQ { get; set; }
}

public class RedisCircuitBreakerSettings
{
    public int ExceptionsAllowedBeforeBreaking { get; set; } = 5;
    public int DurationOfBreakInSeconds { get; set; } = 30;
    public int RetryCount { get; set; } = 3;
    public int RetryDelayInSeconds { get; set; } = 1;
    public int TimeoutInSeconds { get; set; } = 5;
}

public class RabbitMQCircuitBreakerSettings
{
    public int ExceptionsAllowedBeforeBreaking { get; set; } = 3;
    public int DurationOfBreakInSeconds { get; set; } = 60;
    public int RetryCount { get; set; } = 5;
    public int RetryDelayInSeconds { get; set; } = 2;
    public int TimeoutInSeconds { get; set; } = 10;
}
```

#### appsettings.json
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
    }
  }
}
```

### Circuit Breaker Service Implementation

#### CircuitBreakerService
```csharp
public class CircuitBreakerService : ICircuitBreakerService
{
    private readonly ILogger<CircuitBreakerService> _logger;
    private readonly Dictionary<string, IAsyncPolicy> _policies;
    private readonly Dictionary<string, CircuitBreakerState> _states;

    public CircuitBreakerService(ILogger<CircuitBreakerService> logger)
    {
        _logger = logger;
        _policies = new Dictionary<string, IAsyncPolicy>();
        _states = new Dictionary<string, CircuitBreakerState>();
    }

    public void AddPolicy(string serviceKey, IAsyncPolicy policy)
    {
        _policies[serviceKey] = policy;
        _states[serviceKey] = CircuitBreakerState.Closed;
        _logger.LogInformation("Added circuit breaker policy for service: {ServiceKey}", serviceKey);
    }

    public async Task<T> ExecuteAsync<T>(string serviceKey, Func<Task<T>> action, CancellationToken cancellationToken = default)
    {
        if (!_policies.ContainsKey(serviceKey))
        {
            _logger.LogWarning("No circuit breaker policy found for service: {ServiceKey}, executing without circuit breaker", serviceKey);
            return await action();
        }

        try
        {
            var result = await _policies[serviceKey].ExecuteAsync(async () => await action());
            _states[serviceKey] = CircuitBreakerState.Closed;
            return result;
        }
        catch (BrokenCircuitException)
        {
            _states[serviceKey] = CircuitBreakerState.Open;
            _logger.LogWarning("Circuit breaker is OPEN for service: {ServiceKey}", serviceKey);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing action for service: {ServiceKey}", serviceKey);
            throw;
        }
    }

    public async Task ExecuteAsync(string serviceKey, Func<Task> action, CancellationToken cancellationToken = default)
    {
        if (!_policies.ContainsKey(serviceKey))
        {
            _logger.LogWarning("No circuit breaker policy found for service: {ServiceKey}, executing without circuit breaker", serviceKey);
            await action();
            return;
        }

        try
        {
            await _policies[serviceKey].ExecuteAsync(async () => await action());
            _states[serviceKey] = CircuitBreakerState.Closed;
        }
        catch (BrokenCircuitException)
        {
            _states[serviceKey] = CircuitBreakerState.Open;
            _logger.LogWarning("Circuit breaker is OPEN for service: {ServiceKey}", serviceKey);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing action for service: {ServiceKey}", serviceKey);
            throw;
        }
    }

    public CircuitBreakerState GetState(string serviceKey)
    {
        return _states.ContainsKey(serviceKey) ? _states[serviceKey] : CircuitBreakerState.Closed;
    }
}
```

### Circuit Breaker Policy Factory

#### CircuitBreakerPolicyFactory
```csharp
public static class CircuitBreakerPolicyFactory
{
    public static IAsyncPolicy CreateCircuitBreakerPolicy(
        string serviceKey,
        int exceptionsAllowedBeforeBreaking,
        TimeSpan durationOfBreak,
        ILogger logger)
    {
        return Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: exceptionsAllowedBeforeBreaking,
                durationOfBreak: durationOfBreak,
                onBreak: (exception, breakDelay) =>
                {
                    logger.LogWarning("Circuit breaker OPEN for {ServiceKey} after {ExceptionsAllowed} exceptions. Duration: {Duration}s",
                        serviceKey, exceptionsAllowedBeforeBreaking, breakDelay.TotalSeconds);
                },
                onReset: () =>
                {
                    logger.LogInformation("Circuit breaker RESET for {ServiceKey}", serviceKey);
                },
                onHalfOpen: () =>
                {
                    logger.LogInformation("Circuit breaker HALF-OPEN for {ServiceKey}", serviceKey);
                });
    }

    public static IAsyncPolicy CreateRetryPolicy(
        string serviceKey,
        int retryCount,
        TimeSpan retryDelay,
        ILogger logger)
    {
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: retryCount,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) * retryDelay.TotalSeconds),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning("Retry {RetryCount}/{MaxRetries} for {ServiceKey} after {Delay}s due to: {Exception}",
                        retryCount, retryCount, serviceKey, timeSpan.TotalSeconds, exception.Message);
                });
    }

    public static IAsyncPolicy CreateTimeoutPolicy(
        string serviceKey,
        TimeSpan timeout,
        ILogger logger)
    {
        return Policy
            .TimeoutAsync(
                timeout: timeout,
                timeoutStrategy: Polly.Timeout.TimeoutStrategy.Optimistic,
                onTimeout: (context, timeSpan, task) =>
                {
                    logger.LogWarning("Timeout occurred for {ServiceKey} after {Timeout}s", serviceKey, timeSpan.TotalSeconds);
                });
    }
}
```

### Circuit Breaker Service Wrappers

#### CircuitBreakerCacheService
```csharp
public class CircuitBreakerCacheService : ICacheService
{
    private readonly ICacheService _innerCacheService;
    private readonly ICircuitBreakerService _circuitBreakerService;
    private readonly ILogger<CircuitBreakerCacheService> _logger;
    private const string ServiceKey = "RedisCache";

    public CircuitBreakerCacheService(
        ICacheService innerCacheService,
        ICircuitBreakerService circuitBreakerService,
        ILogger<CircuitBreakerCacheService> logger)
    {
        _innerCacheService = innerCacheService;
        _circuitBreakerService = circuitBreakerService;
        _logger = logger;
    }

    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _circuitBreakerService.ExecuteAsync(ServiceKey, async () =>
            {
                return await _innerCacheService.GetAsync<T>(key, cancellationToken);
            }, cancellationToken);
        }
        catch (BrokenCircuitException)
        {
            _logger.LogWarning("Circuit breaker is open for Redis cache, skipping cache read for key: {Key}", key);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in cache service GetAsync for key: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await _circuitBreakerService.ExecuteAsync(ServiceKey, async () =>
            {
                await _innerCacheService.SetAsync(key, value, expiration, cancellationToken);
            }, cancellationToken);
        }
        catch (BrokenCircuitException)
        {
            _logger.LogWarning("Circuit breaker is open for Redis cache, skipping cache write for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in cache service SetAsync for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _circuitBreakerService.ExecuteAsync(ServiceKey, async () =>
            {
                await _innerCacheService.RemoveAsync(key, cancellationToken);
            }, cancellationToken);
        }
        catch (BrokenCircuitException)
        {
            _logger.LogWarning("Circuit breaker is open for Redis cache, skipping cache removal for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in cache service RemoveAsync for key: {Key}", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            await _circuitBreakerService.ExecuteAsync(ServiceKey, async () =>
            {
                await _innerCacheService.RemoveByPatternAsync(pattern, cancellationToken);
            }, cancellationToken);
        }
        catch (BrokenCircuitException)
        {
            _logger.LogWarning("Circuit breaker is open for Redis cache, skipping cache pattern removal: {Pattern}", pattern);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in cache service RemoveByPatternAsync for pattern: {Pattern}", pattern);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _circuitBreakerService.ExecuteAsync(ServiceKey, async () =>
            {
                return await _innerCacheService.ExistsAsync(key, cancellationToken);
            }, cancellationToken);
        }
        catch (BrokenCircuitException)
        {
            _logger.LogWarning("Circuit breaker is open for Redis cache, skipping cache existence check for key: {Key}", key);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in cache service ExistsAsync for key: {Key}", key);
            return false;
        }
    }
}
```

#### CircuitBreakerMessagePublisher
```csharp
public class CircuitBreakerMessagePublisher : IMessagePublisher
{
    private readonly IMessagePublisher _innerMessagePublisher;
    private readonly ICircuitBreakerService _circuitBreakerService;
    private readonly ILogger<CircuitBreakerMessagePublisher> _logger;
    private const string ServiceKey = "RabbitMQ";

    public CircuitBreakerMessagePublisher(
        IMessagePublisher innerMessagePublisher,
        ICircuitBreakerService circuitBreakerService,
        ILogger<CircuitBreakerMessagePublisher> logger)
    {
        _innerMessagePublisher = innerMessagePublisher;
        _circuitBreakerService = circuitBreakerService;
        _logger = logger;
    }

    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class
    {
        try
        {
            await _circuitBreakerService.ExecuteAsync(ServiceKey, async () =>
            {
                await _innerMessagePublisher.PublishAsync(message, cancellationToken);
            }, cancellationToken);
        }
        catch (BrokenCircuitException)
        {
            _logger.LogWarning("Circuit breaker is open for RabbitMQ, skipping message publish for message type: {MessageType}", 
                message?.GetType().Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in message publisher PublishAsync for message type: {MessageType}", 
                message?.GetType().Name);
        }
    }

    public async Task PublishAsync<TMessage>(IEnumerable<TMessage> messages, CancellationToken cancellationToken = default) where TMessage : class
    {
        try
        {
            await _circuitBreakerService.ExecuteAsync(ServiceKey, async () =>
            {
                await _innerMessagePublisher.PublishAsync(messages, cancellationToken);
            }, cancellationToken);
        }
        catch (BrokenCircuitException)
        {
            _logger.LogWarning("Circuit breaker is open for RabbitMQ, skipping batch message publish for {MessageCount} messages", 
                messages?.Count() ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in message publisher PublishAsync for batch of {MessageCount} messages", 
                messages?.Count() ?? 0);
        }
    }

    public async Task PublishAsync(object entity, string messageType, string messageAction, CancellationToken cancellationToken = default)
    {
        try
        {
            await _circuitBreakerService.ExecuteAsync(ServiceKey, async () =>
            {
                await _innerMessagePublisher.PublishAsync(entity, messageType, messageAction, cancellationToken);
            }, cancellationToken);
        }
        catch (BrokenCircuitException)
        {
            _logger.LogWarning("Circuit breaker is open for RabbitMQ, skipping message publish for type: {MessageType}, action: {MessageAction}", 
                messageType, messageAction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in message publisher PublishAsync for type: {MessageType}, action: {MessageAction}", 
                messageType, messageAction);
        }
    }
}
```

### Health Monitoring

#### CircuitBreakerHealthController
```csharp
[ApiController]
[Route("api/[controller]")]
public class CircuitBreakerHealthController : ControllerBase
{
    private readonly ICircuitBreakerService _circuitBreakerService;
    private readonly ILogger<CircuitBreakerHealthController> _logger;

    public CircuitBreakerHealthController(
        ICircuitBreakerService circuitBreakerService,
        ILogger<CircuitBreakerHealthController> logger)
    {
        _circuitBreakerService = circuitBreakerService;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult GetCircuitBreakerHealth()
    {
        var redisState = _circuitBreakerService.GetState("RedisCache");
        var rabbitMQState = _circuitBreakerService.GetState("RabbitMQ");

        var healthStatus = new
        {
            Timestamp = DateTime.UtcNow,
            Services = new
            {
                RedisCache = new
                {
                    State = redisState.ToString(),
                    IsHealthy = redisState == CircuitBreakerState.Closed
                },
                RabbitMQ = new
                {
                    State = rabbitMQState.ToString(),
                    IsHealthy = rabbitMQState == CircuitBreakerState.Closed
                }
            },
            OverallHealth = redisState == CircuitBreakerState.Closed && rabbitMQState == CircuitBreakerState.Closed
        };

        _logger.LogInformation("Circuit breaker health check: Redis={RedisState}, RabbitMQ={RabbitMQState}", 
            redisState, rabbitMQState);

        return Ok(healthStatus);
    }

    [HttpGet("{serviceKey}")]
    public ActionResult GetServiceCircuitBreakerHealth(string serviceKey)
    {
        var state = _circuitBreakerService.GetState(serviceKey);
        
        var serviceHealth = new
        {
            ServiceKey = serviceKey,
            State = state.ToString(),
            IsHealthy = state == CircuitBreakerState.Closed,
            Timestamp = DateTime.UtcNow
        };

        _logger.LogInformation("Circuit breaker health check for {ServiceKey}: {State}", serviceKey, state);

        return Ok(serviceHealth);
    }
}
```

### Dependency Injection

```csharp
// Configure Circuit Breaker settings
services.Configure<CircuitBreakerSettings>(
    configuration.GetSection("CircuitBreaker"));

// Configure and register Circuit Breaker service
services.AddSingleton<ICircuitBreakerService>(sp =>
{
    var circuitBreakerService = new CircuitBreakerService(sp.GetRequiredService<ILogger<CircuitBreakerService>>());
    var circuitBreakerSettings = configuration.GetSection("CircuitBreaker").Get<CircuitBreakerSettings>();
    var logger = sp.GetRequiredService<ILogger<CircuitBreakerService>>();

    if (circuitBreakerSettings?.Redis != null)
    {
        var redisPolicy = CircuitBreakerPolicyFactory.CreateCircuitBreakerPolicy(
            "RedisCache",
            circuitBreakerSettings.Redis.ExceptionsAllowedBeforeBreaking,
            TimeSpan.FromSeconds(circuitBreakerSettings.Redis.DurationOfBreakInSeconds),
            logger);

        var redisRetryPolicy = CircuitBreakerPolicyFactory.CreateRetryPolicy(
            "RedisCache",
            circuitBreakerSettings.Redis.RetryCount,
            TimeSpan.FromSeconds(circuitBreakerSettings.Redis.RetryDelayInSeconds),
            logger);

        var redisTimeoutPolicy = CircuitBreakerPolicyFactory.CreateTimeoutPolicy(
            "RedisCache",
            TimeSpan.FromSeconds(circuitBreakerSettings.Redis.TimeoutInSeconds),
            logger);

        var combinedRedisPolicy = Policy.WrapAsync(redisTimeoutPolicy, redisRetryPolicy, redisPolicy);
        circuitBreakerService.AddPolicy("RedisCache", combinedRedisPolicy);
    }

    if (circuitBreakerSettings?.RabbitMQ != null)
    {
        var rabbitMQPolicy = CircuitBreakerPolicyFactory.CreateCircuitBreakerPolicy(
            "RabbitMQ",
            circuitBreakerSettings.RabbitMQ.ExceptionsAllowedBeforeBreaking,
            TimeSpan.FromSeconds(circuitBreakerSettings.RabbitMQ.DurationOfBreakInSeconds),
            logger);

        var rabbitMQRetryPolicy = CircuitBreakerPolicyFactory.CreateRetryPolicy(
            "RabbitMQ",
            circuitBreakerSettings.RabbitMQ.RetryCount,
            TimeSpan.FromSeconds(circuitBreakerSettings.RabbitMQ.RetryDelayInSeconds),
            logger);

        var rabbitMQTimeoutPolicy = CircuitBreakerPolicyFactory.CreateTimeoutPolicy(
            "RabbitMQ",
            TimeSpan.FromSeconds(circuitBreakerSettings.RabbitMQ.TimeoutInSeconds),
            logger);

        var combinedRabbitMQPolicy = Policy.WrapAsync(rabbitMQTimeoutPolicy, rabbitMQRetryPolicy, rabbitMQPolicy);
        circuitBreakerService.AddPolicy("RabbitMQ", combinedRabbitMQPolicy);
    }

    return circuitBreakerService;
});
```

## Circuit Breaker Policies

### Policy Composition

#### Redis Circuit Breaker Policy
- **Exceptions Allowed**: 5
- **Break Duration**: 30 seconds
- **Retry Count**: 3
- **Retry Delay**: 1 second (exponential backoff)
- **Timeout**: 5 seconds

#### RabbitMQ Circuit Breaker Policy
- **Exceptions Allowed**: 3
- **Break Duration**: 60 seconds
- **Retry Count**: 5
- **Retry Delay**: 2 seconds (exponential backoff)
- **Timeout**: 10 seconds

### Policy Order
1. **Timeout Policy**: Ensures operations don't hang indefinitely
2. **Retry Policy**: Retries failed operations with exponential backoff
3. **Circuit Breaker Policy**: Opens circuit after too many failures

## Testing Guidelines

### Unit Tests
- Test circuit breaker state transitions
- Test policy factory methods
- Test retry logic with exponential backoff
- Test timeout enforcement
- Test service wrapper behavior

### Integration Tests
- Test circuit breaker state transitions in real scenarios
- Test fallback mechanisms
- Test circuit breaker recovery
- Test cascading failure prevention
- Test health monitoring

### Performance Tests
- Test circuit breaker overhead
- Test retry policy performance
- Test timeout enforcement
- Test with high failure rates
- Measure impact on API performance

## Best Practices

### Circuit Breaker Configuration
- Set appropriate thresholds for each service
- Configure reasonable break durations
- Tune retry parameters based on service characteristics
- Monitor circuit breaker states
- Adjust parameters based on metrics

### Error Handling
- Log all circuit breaker events
- Implement proper fallback mechanisms
- Don't expose internal errors to clients
- Monitor circuit breaker health
- Implement alerting for critical states

### Monitoring
- Track circuit breaker state changes
- Monitor success/failure rates
- Alert on circuit breaker opens
- Track recovery times
- Monitor policy effectiveness

## Troubleshooting

### Common Issues

#### Circuit Breaker Not Opening
- **Symptom**: Circuit breaker remains closed despite failures
- **Solution**: Check exception threshold, policy configuration
- **Check**: Review logs for circuit breaker events

#### Circuit Breaker Not Recovering
- **Symptom**: Circuit breaker stays open after service recovery
- **Solution**: Check break duration, half-open behavior
- **Check**: Verify service is actually healthy

#### Too Many Retries
- **Symptom**: Excessive retry attempts delaying failures
- **Solution**: Adjust retry count and delay parameters
- **Check**: Review retry policy configuration

#### Timeout Issues
- **Symptom**: Operations timing out prematurely
- **Solution**: Adjust timeout settings based on service characteristics
- **Check**: Monitor operation durations

## Performance Considerations

### Expected Performance
- Circuit breaker overhead: < 1ms
- Retry delay: Configurable exponential backoff
- Timeout enforcement: Accurate to configured values
- State transition: Instantaneous

### Optimization Strategies
- Monitor circuit breaker overhead
- Tune thresholds based on metrics
- Use appropriate break durations
- Optimize retry parameters
- Monitor timeout effectiveness

## Security Considerations

### Security Aspects
- Circuit breaker doesn't introduce security vulnerabilities
- Monitor for circuit breaker abuse
- Protect health check endpoints
- Audit circuit breaker events
- Implement proper access controls

## Monitoring and Observability

### Metrics to Track
- Circuit breaker state transitions
- Success/failure rates per service
- Retry attempts and successes
- Timeout occurrences
- Recovery times

### Logging
- Log all circuit breaker state changes
- Log retry attempts and outcomes
- Log timeout events
- Log fallback activations
- Log policy configuration changes

### Health Checks
- Circuit breaker state monitoring
- Service health correlation
- Policy effectiveness monitoring
- Recovery time tracking
- Alert on critical states

## Success Criteria

- [x] Circuit breaker implemented for all external services
- [x] Retry logic with exponential backoff working
- [x] Fallback mechanisms functioning correctly
- [x] Circuit breaker state monitoring operational
- [x] No cascading failures during service outages
- [x] Circuit breaker health monitoring functional
- [x] Circuit breaker recovery working properly

## Dependencies

- Polly (8.4.0)
- Microsoft.Extensions.Http.Polly (10.0.11)

## Related Documentation

- [AGENTS.md](./AGENTS.md) - AI Agent Guidelines
- [ARCHITECTURE.md](./ARCHITECTURE.md) - Architecture Documentation
- [DEVELOPMENT_GUIDELINES.md](./DEVELOPMENT_GUIDELINES.md) - Development Guidelines
- [PROJECT_SPEC.md](./PROJECT_SPEC.md) - Project Specification
- [PHASE_1_REDIS_CACHE_SPEC.md](./PHASE_1_REDIS_CACHE_SPEC.md) - Redis Cache Specification
- [PHASE_2_RABBITMQ_MESSAGING_SPEC.md](./PHASE_2_RABBITMQ_MESSAGING_SPEC.md) - RabbitMQ Messaging Specification
