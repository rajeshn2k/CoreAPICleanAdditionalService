using Core.Library.Clean.AdditionalService.ResilientTest;
using Microsoft.Extensions.Logging;

namespace Core.Library.Clean.AdditionalService
{
    public class InMemoryCacheService : ICacheService
    {
        private readonly Dictionary<string, CacheEntry> _cache = new();
        private readonly ILogger<InMemoryCacheService> _logger;
        private readonly ResilientTestService resilientTestService;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        //e-exception, t-timeout, a-default
        private readonly string testMode = "a";

        public InMemoryCacheService(
            ILogger<InMemoryCacheService> logger, ResilientTestService resilientTestService)
        {
            _logger = logger;
            this.resilientTestService = resilientTestService;
        }


        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(cancellationToken);

            try
            {
                await resilientTestService.InjectIssueDelayExceptionNone(testMode, cancellationToken);

                if (_cache.TryGetValue(key, out var entry))
                {
                    if (entry.Expiration > DateTime.UtcNow)
                    {
                        _logger.LogDebug("In-memory cache hit for key: {Key}", key);
                        return (T)entry.Value;
                    }
                    else
                    {
                        _cache.Remove(key);
                    }
                }
                return default(T);
            }
            /* Having 'NO' exception handler here will let Resilience Pipelin to enable the statergy
              * Polly Resilience Pipelin Implements staergy or policies such as Retry, Circuit breaker, TimeOut
              * ResilientCacheService is the caller or excuter for actual InMemoryCacheService or RedisCacheService*/
            finally
            {
                _lock.Release();
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(cancellationToken);
            try
            {
                await resilientTestService.InjectIssueDelayExceptionNone(testMode, cancellationToken);

                var expiry = expiration ?? TimeSpan.FromMinutes(30);
                _cache[key] = new CacheEntry
                {
                    Value = value,
                    Expiration = DateTime.UtcNow.Add(expiry)
                };
                _logger.LogDebug("In-memory cache set for key: {Key}", key);
            }
            /* Having 'NO' exception handler here will let Resilience Pipelin to enable the statergy
              * Polly Resilience Pipelin Implements staergy or policies such as Retry, Circuit breaker, TimeOut
              * ResilientCacheService is the caller or excuter for actual InMemoryCacheService or RedisCacheService*/
            finally
            {
                _lock.Release();
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(cancellationToken);
            try
            {
                await resilientTestService.InjectIssueDelayExceptionNone(testMode, cancellationToken);

                _cache.Remove(key);
                _logger.LogDebug("In-memory cache removed for key: {Key}", key);
            }
            /* Having 'NO' exception handler here will let Resilience Pipelin to enable the statergy
              * Polly Resilience Pipelin Implements staergy or policies such as Retry, Circuit breaker, TimeOut
              * ResilientCacheService is the caller or excuter for actual InMemoryCacheService or RedisCacheService*/
            finally
            {
                _lock.Release();
            }
        }

        public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(cancellationToken);
            try
            {
                await resilientTestService.InjectIssueDelayExceptionNone(testMode, cancellationToken);

                var keysToRemove = _cache.Keys.Where(k => k.Contains(pattern)).ToList();
                foreach (var key in keysToRemove)
                {
                    _cache.Remove(key);
                }
                _logger.LogDebug("In-memory cache removed {Count} keys matching pattern: {Pattern}", keysToRemove.Count, pattern);
            }
            /* Having 'NO' exception handler here will let Resilience Pipelin to enable the statergy
              * Polly Resilience Pipelin Implements staergy or policies such as Retry, Circuit breaker, TimeOut
              * ResilientCacheService is the caller or excuter for actual InMemoryCacheService or RedisCacheService*/
            finally
            {
                _lock.Release();
            }
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(cancellationToken);
            try
            {
                await resilientTestService.InjectIssueDelayExceptionNone(testMode, cancellationToken);

                if (_cache.TryGetValue(key, out var entry))
                {
                    if (entry.Expiration > DateTime.UtcNow)
                    {
                        return true;
                    }
                    else
                    {
                        _cache.Remove(key);
                    }
                }
                return false;
            }
            /* Having 'NO' exception handler here will let Resilience Pipelin to enable the statergy
             * Polly Resilience Pipelin Implements staergy or policies such as Retry, Circuit breaker, TimeOut
             * ResilientCacheService is the caller or excuter for actual InMemoryCacheService or RedisCacheService*/
            finally
            {
                _lock.Release();
            }
        }

        public async Task RemoveAllByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            await RemoveByPatternAsync(pattern, cancellationToken);
        }

        private class CacheEntry
        {
            public object Value { get; set; }
            public DateTime Expiration { get; set; }
        }
    }
}