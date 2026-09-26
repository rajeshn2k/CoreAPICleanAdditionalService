using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Core.Library.Clean.AdditionalService;

namespace Core.API.Clean.AdditionalService.RateLimiting
{
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
}