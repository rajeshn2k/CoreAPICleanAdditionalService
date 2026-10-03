//WORKING - UNCOMMENT FOR TESTING
/*
namespace Core.Library.Clean.AdditionalService
{
using StackExchange.Redis;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

    /// <summary>
    /// Redis implementation of cache service
    /// CircuitBreakerCacheService will handle the exception
    /// Having exception handler here will force, Retry won't happen and Circuit breaker won't open
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly ILogger<RedisCacheService> _logger;
        
        public RedisCacheService(
            IConnectionMultiplexer redis,
            ILogger<RedisCacheService> logger)
        {
            _redis = redis;
            _database = redis.GetDatabase();
            _logger = logger;
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var value = await _database.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                return default(T);
            }

            var deserialized = JsonConvert.DeserializeObject<T>(value.ToString());
            _logger.LogDebug("Cache hit for key: {Key}", key);
            return deserialized;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            var serialized = JsonConvert.SerializeObject(value);
            var expiry = expiration ?? TimeSpan.FromMinutes(30);

            await _database.StringSetAsync(key, serialized, expiry);
            _logger.LogDebug("Cache set for key: {Key} with expiration: {Expiration}", key, expiry);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _database.KeyDeleteAsync(key);
            _logger.LogDebug("Cache removed for key: {Key}", key);
        }

        public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern).ToArray();

            if (keys.Length > 0)
            {
                await _database.KeyDeleteAsync(keys);
                _logger.LogDebug("Cache removed {Count} keys matching pattern: {Pattern}", keys.Length, pattern);
            }
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _database.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if key exists in cache: {Key}", key);
                return false;
            }
        }

        public async Task RemoveAllByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            await RemoveByPatternAsync(pattern, cancellationToken);
        }
    }
}

*/