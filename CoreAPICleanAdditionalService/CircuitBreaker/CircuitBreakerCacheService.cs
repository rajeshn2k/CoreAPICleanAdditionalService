using Core.Library.Clean.AdditionalService;
using Microsoft.Extensions.Logging;

namespace Core.API.Clean.AdditionalService.CircuitBreaker
{
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
}