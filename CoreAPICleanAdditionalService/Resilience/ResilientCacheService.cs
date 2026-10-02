using Core.Library.Clean.AdditionalService;
using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using Polly.Timeout;

namespace Core.API.Clean.AdditionalService.Resilience
{
    /// <summary>
    /// Resilient decorator for ICacheService using Microsoft.Extensions.Resilience
    /// </summary>
    public sealed class ResilientCacheService : ICacheService
    {
        private const string PipelineKey = "Redis";
        private readonly ICacheService _innerCacheService;
        private readonly ResiliencePipeline _pipeline;
        private readonly ILogger<ResilientCacheService> _logger;

        public ResilientCacheService(
            ICacheService innerCacheService,
            ResiliencePipelineProvider<string> pipelineProvider,
            ILogger<ResilientCacheService> logger)
        {
            _innerCacheService = innerCacheService;
            _pipeline = pipelineProvider.GetPipeline(PipelineKey);
            _logger = logger;
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _pipeline.ExecuteAsync(
                    async token =>
                    {
                        return await _innerCacheService.GetAsync<T>(key, token);
                    },
                    cancellationToken);
            }
            /*
             * ONLY handle Exception produced by Polly Resilience Pipelin Implements staergy
             * policies such as Retry, Circuit breaker, TimeOut (Expected Behaviour)
             * 1.TimeoutRejectedException,2.BrokenCircuitException
             * Resilience - application's ability to withstand and rapidly recover from disruptions, failures
             * Expectation is failure due to InMemoryCacheService or RedisCacheService should not break API data access
             */
            catch (TimeoutRejectedException ex)
            {
                //_logger.LogWarning(ex, "Cache operation timed out for key: {Key}", key);
                _logger.LogWarning("Cache operation timed out for key: {Key}", key);
                return default;
            }
            catch (BrokenCircuitException ex)
            {
                //_logger.LogWarning(ex, "Redis circuit breaker is open. Cache read skipped for key: {Key}", key);
                _logger.LogWarning("Redis circuit breaker is open. Cache read skipped for key: {Key}", key);
                return default;
            }
            /*
             * it will catch essentially any exception that escapes _pipeline.ExecuteAsync(...) and turn it into a CACHE MISS by returning default
             * So if _innerCacheService.GetAsync<T>() throws something unexpected, such as:
             * 1.RedisConnectionException 2.RedisTimeoutException 3.NullReferenceException 4.ArgumentException 5.InvalidOperationException 6.OperationCanceledException 7.OutOfMemoryException
             * IF YOU WANT CancellationToken PROPAGATE BACK TO DIRECTOR, THIS WILL PREVENT
             */
            catch (Exception ex)
            {
                //_logger.LogWarning(ex, "UN-HANDLED EXCEPTION swallowing programming bugs and cancellation: {Key}", key);
                _logger.LogWarning("UN-HANDLED EXCEPTION swallowing programming bugs and cancellation");
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            try
            {
                await _pipeline.ExecuteAsync(
                    async token =>
                    {
                        await _innerCacheService.SetAsync(key, value, expiration, token);
                    },
                    cancellationToken);
            }
            /*
            * ONLY handle Exception produced by Polly Resilience Pipelin Implements staergy
            * policies such as Retry, Circuit breaker, TimeOut (Expected Behaviour)
            * 1.TimeoutRejectedException,2.BrokenCircuitException
            * Resilience - application's ability to withstand and rapidly recover from disruptions, failures
            * Expectation is failure due to InMemoryCacheService or RedisCacheService should not break API data access
            */
            catch (TimeoutRejectedException ex)
            {
                //_logger.LogWarning(ex, "Cache operation timed out for key: {Key}", key);
                _logger.LogWarning("Cache operation timed out for key: {Key}", key);
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogWarning("Redis circuit breaker is open. Cache write skipped for key: {Key}", key);
                //_logger.LogWarning(ex, "Redis circuit breaker is open. Cache write skipped for key: {Key}", key);
            }
            /*
             * it will catch essentially any exception that escapes _pipeline.ExecuteAsync(...) and turn it into a CACHE MISS by returning default
             * So if _innerCacheService.GetAsync<T>() throws something unexpected, such as:
             * 1.RedisConnectionException 2.RedisTimeoutException 3.NullReferenceException 4.ArgumentException 5.InvalidOperationException 6.OperationCanceledException 7.OutOfMemoryException
             * IF YOU WANT CancellationToken PROPAGATE BACK TO DIRECTOR, THIS WILL PREVENT
             */
            catch (Exception ex)
            {
                //_logger.LogWarning(ex, "UN-HANDLED EXCEPTION swallowing programming bugs and cancellation: {Key}", key);
                _logger.LogWarning("UN-HANDLED EXCEPTION swallowing programming bugs and cancellation");
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _pipeline.ExecuteAsync(
                    async token =>
                    {
                        await _innerCacheService.RemoveAsync(key, token);
                    },
                    cancellationToken);
            }
            /*
            * ONLY handle Exception produced by Polly Resilience Pipelin Implements staergy
            * policies such as Retry, Circuit breaker, TimeOut (Expected Behaviour)
            * 1.TimeoutRejectedException,2.BrokenCircuitException
            * Resilience - application's ability to withstand and rapidly recover from disruptions, failures
            * Expectation is failure due to InMemoryCacheService or RedisCacheService should not break API data access
            */
            catch (TimeoutRejectedException ex)
            {
                //_logger.LogWarning(ex, "Cache operation timed out for key: {Key}", key);
                _logger.LogWarning("Cache operation timed out for key: {Key}", key);
            }
            catch (BrokenCircuitException ex)
            {
                //_logger.LogWarning(ex, "Redis circuit breaker is open. Cache removal skipped for key: {Key}", key);
                _logger.LogWarning("Redis circuit breaker is open. Cache removal skipped for key: {Key}", key);
            }
            /*
             * it will catch essentially any exception that escapes _pipeline.ExecuteAsync(...) and turn it into a CACHE MISS by returning default
             * So if _innerCacheService.GetAsync<T>() throws something unexpected, such as:
             * 1.RedisConnectionException 2.RedisTimeoutException 3.NullReferenceException 4.ArgumentException 5.InvalidOperationException 6.OperationCanceledException 7.OutOfMemoryException
             * IF YOU WANT CancellationToken PROPAGATE BACK TO DIRECTOR, THIS WILL PREVENT
             */
            catch (Exception ex)
            {
                //_logger.LogWarning(ex, "UN-HANDLED EXCEPTION swallowing programming bugs and cancellation: {Key}", key);
                _logger.LogWarning("UN-HANDLED EXCEPTION swallowing programming bugs and cancellation");
            }
        }

        public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            try
            {
                await _pipeline.ExecuteAsync(
                    async token =>
                    {
                        await _innerCacheService.RemoveByPatternAsync(pattern, token);
                    },
                    cancellationToken);
            }
            /*
            * ONLY handle Exception produced by Polly Resilience Pipelin Implements staergy
            * policies such as Retry, Circuit breaker, TimeOut (Expected Behaviour)
            * 1.TimeoutRejectedException,2.BrokenCircuitException
            * Resilience - application's ability to withstand and rapidly recover from disruptions, failures
            * Expectation is failure due to InMemoryCacheService or RedisCacheService should not break API data access
            */
            catch (TimeoutRejectedException ex)
            {
                //_logger.LogWarning(ex, "Cache operation timed out for key: {Key}", key);
                _logger.LogWarning("Cache operation timed out");
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogWarning("Redis circuit breaker is open. Cache pattern removal skipped: {Pattern}", pattern);
                //_logger.LogWarning(ex, "Redis circuit breaker is open. Cache pattern removal skipped: {Pattern}", pattern);
            }
            /*
             * it will catch essentially any exception that escapes _pipeline.ExecuteAsync(...) and turn it into a CACHE MISS by returning default
             * So if _innerCacheService.GetAsync<T>() throws something unexpected, such as:
             * 1.RedisConnectionException 2.RedisTimeoutException 3.NullReferenceException 4.ArgumentException 5.InvalidOperationException 6.OperationCanceledException 7.OutOfMemoryException
             * IF YOU WANT CancellationToken PROPAGATE BACK TO DIRECTOR, THIS WILL PREVENT
             */
            catch (Exception ex)
            {
                //_logger.LogWarning(ex, "UN-HANDLED EXCEPTION swallowing programming bugs and cancellation: {Key}", key);
                _logger.LogWarning("UN-HANDLED EXCEPTION swallowing programming bugs and cancellation");
            }
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _pipeline.ExecuteAsync(
                    async token =>
                    {
                        return await _innerCacheService.ExistsAsync(key, token);
                    },
                    cancellationToken);
            }
            /*
            * ONLY handle Exception produced by Polly Resilience Pipelin Implements staergy
            * policies such as Retry, Circuit breaker, TimeOut (Expected Behaviour)
            * 1.TimeoutRejectedException,2.BrokenCircuitException
            * Resilience - application's ability to withstand and rapidly recover from disruptions, failures
            * Expectation is failure due to InMemoryCacheService or RedisCacheService should not break API data access
            */
            catch (TimeoutRejectedException ex)
            {
                //_logger.LogWarning(ex, "Cache operation timed out for key: {Key}", key);
                _logger.LogWarning("Cache operation timed out for key: {Key}", key);
                return false;
            }
            catch (BrokenCircuitException ex)
            {
                //_logger.LogWarning(ex, "Redis circuit breaker is open. Cache existence check skipped for key: {Key}", key);
                _logger.LogWarning("Redis circuit breaker is open. Cache existence check skipped for key: {Key}", key);
                return false;
            }
            /*
             * it will catch essentially any exception that escapes _pipeline.ExecuteAsync(...) and turn it into a CACHE MISS by returning default
             * So if _innerCacheService.GetAsync<T>() throws something unexpected, such as:
             * 1.RedisConnectionException 2.RedisTimeoutException 3.NullReferenceException 4.ArgumentException 5.InvalidOperationException 6.OperationCanceledException 7.OutOfMemoryException
             * IF YOU WANT CancellationToken PROPAGATE BACK TO DIRECTOR, THIS WILL PREVENT
             */
            catch (Exception ex)
            {
                //_logger.LogWarning(ex, "UN-HANDLED EXCEPTION swallowing programming bugs and cancellation: {Key}", key);
                _logger.LogWarning("UN-HANDLED EXCEPTION swallowing programming bugs and cancellation");
                return default;
            }
        }

        public async Task RemoveAllByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            try
            {
                await _pipeline.ExecuteAsync(
                    async token =>
                    {
                        await _innerCacheService.RemoveAllByPatternAsync(pattern, token);
                    },
                    cancellationToken);
            }
            /*
            * ONLY handle Exception produced by Polly Resilience Pipelin Implements staergy
            * policies such as Retry, Circuit breaker, TimeOut (Expected Behaviour)
            * 1.TimeoutRejectedException,2.BrokenCircuitException
            * Resilience - application's ability to withstand and rapidly recover from disruptions, failures
            * Expectation is failure due to InMemoryCacheService or RedisCacheService should not break API data access
            */
            catch (TimeoutRejectedException ex)
            {
                //_logger.LogWarning(ex, "Cache operation timed out for key: {Key}", key);
                _logger.LogWarning("Cache operation timed out");
            }
            catch (BrokenCircuitException ex)
            {
                //_logger.LogWarning(ex, "Redis circuit breaker is open. Cache pattern removal skipped: {Pattern}", pattern);
                _logger.LogWarning("Redis circuit breaker is open. Cache pattern removal skipped: {Pattern}", pattern);
            }
            /*
             * it will catch essentially any exception that escapes _pipeline.ExecuteAsync(...) and turn it into a CACHE MISS by returning default
             * So if _innerCacheService.GetAsync<T>() throws something unexpected, such as:
             * 1.RedisConnectionException 2.RedisTimeoutException 3.NullReferenceException 4.ArgumentException 5.InvalidOperationException 6.OperationCanceledException 7.OutOfMemoryException
             * IF YOU WANT CancellationToken PROPAGATE BACK TO DIRECTOR, THIS WILL PREVENT
             */
            catch (Exception ex)
            {
                //_logger.LogWarning(ex, "UN-HANDLED EXCEPTION swallowing programming bugs and cancellation: {Key}", key);
                _logger.LogWarning("UN-HANDLED EXCEPTION swallowing programming bugs and cancellation");
            }
        }
    }
}
