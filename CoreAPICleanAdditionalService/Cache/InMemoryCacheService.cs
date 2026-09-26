using Core.Library.Clean.AdditionalService;
using Microsoft.Extensions.Options;

namespace Core.API.Clean.AdditionalService.Cache
{
    /// <summary>
    /// In-memory fallback cache service when Redis is unavailable
    /// </summary>
    public class InMemoryCacheService : ICacheService
    {
        private readonly Dictionary<string, CacheEntry> _cache = new();
        private readonly ILogger<InMemoryCacheService> _logger;
        private readonly CacheSettings _settings;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public InMemoryCacheService(
            IOptions<CacheSettings> settings,
            ILogger<InMemoryCacheService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(cancellationToken);
            try
            {
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
                var expiry = expiration ?? _settings.DefaultExpiration;
                _cache[key] = new CacheEntry
                {
                    Value = value,
                    Expiration = DateTime.UtcNow.Add(expiry)
                };
                _logger.LogDebug("In-memory cache set for key: {Key}", key);
            }
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
                _cache.Remove(key);
                _logger.LogDebug("In-memory cache removed for key: {Key}", key);
            }
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
                var keysToRemove = _cache.Keys.Where(k => k.Contains(pattern)).ToList();
                foreach (var key in keysToRemove)
                {
                    _cache.Remove(key);
                }
                _logger.LogDebug("In-memory cache removed {Count} keys matching pattern: {Pattern}", keysToRemove.Count, pattern);
            }
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