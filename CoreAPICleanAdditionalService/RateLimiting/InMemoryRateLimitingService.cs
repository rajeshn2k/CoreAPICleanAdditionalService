using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Core.Library.Clean.AdditionalService;

namespace Core.API.Clean.AdditionalService.RateLimiting
{
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
}