using System.Collections.Concurrent;

namespace Core.API.Clean.AdditionalService
{
    // This class provides an in-memory implementation of rate limiting.
    //
    // 1. "InMemory" means that all rate-limit information is stored inside
    // the memory of the current application instance.
    // 2. If the application restarts, all counters are lost.
    public class InMemoryRateLimitingService : IRateLimitingService
    {
        // Stores rate-limit counters using a unique string key.
        //
        // Example:"user:123" or "ip:192.168.1.10" or "login:user@example.com"
        //
        // ConcurrentDictionary Provides thread-safe collections.
        // ConcurrentDictionary is used instead of Dictionary because
        // multiple HTTP requests can access this collection simultaneously.
        // can access and modify the rate-limit counters at the same time.
        private readonly ConcurrentDictionary<string, RateLimitCounter> _counters;
        private readonly ILogger<InMemoryRateLimitingService> _logger;

        // Timer that periodically executes CleanupExpiredCounters().
        // It prevents expired counters from remaining in memory forever.
        private readonly Timer _cleanupTimer;

        public InMemoryRateLimitingService(
            ILogger<InMemoryRateLimitingService> logger)
        {
            _counters = new ConcurrentDictionary<string, RateLimitCounter>();
            _logger = logger;

            // Create a timer that calls CleanupExpiredCounters.
            //
            // First TimeSpan:
            //     Wait 1 minute before the first cleanup.
            //
            // Second TimeSpan:
            //     After the first cleanup, run it every 1 minute.
            //
            // So the cleanup happens approximately:
            //
            //     Application starts
            //            |
            //          1 minute
            //            |
            //       Cleanup()
            //            |
            //          1 minute
            //            |
            //       Cleanup()
            //            |
            //           ...
            _cleanupTimer = new Timer(
                CleanupExpiredCounters,
                null,
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(1));
        }


        // Removes counters that have passed their expiration time.
        // Timer, calls this method approximately once every minute.
        private void CleanupExpiredCounters(object state)
        {
            // UTC is used instead of local time so that the rate-limit
            // calculation is independent of the server's timezone.
            var now = DateTime.UtcNow;

            // EXPIRED COUNTER
            //  - Find all counters whose expiration time is before "now".
            //  - Creates a separate list so that we can safely iterate
            //     over the keys while TryRemove with thread-safe from the dictionary.

            var expiredKeys = _counters
                .Where(kvp => kvp.Value.Expiry < now)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _counters.TryRemove(key, out _);
            }

            if (expiredKeys.Any())
            {
                _logger.LogDebug(
                    "Cleaned up {Count} expired rate limit counters",
                    expiredKeys.Count);
            }
        }


        // Determines whether a request should be allowed.
        // Updates counter for a given key and decides request should be allowed it
        public async Task<bool> IsAllowedAsync(
            string key,
            int limit,
            TimeSpan period,
            CancellationToken cancellationToken = default)
        {
            // Add a new counter if this key doesn't already exist.
            // OR
            // Update the existing counter if the key already exists.
          
            var counter = _counters.AddOrUpdate(

                key,

                // ---------------------------------------------------------
                // ADD FACTORY - This function executes when "key" does NOT exist.
                // ---------------------------------------------------------
                // A new counter is created for the KEY
                _ => new RateLimitCounter
                {
                    Count = 1,
                    Expiry = DateTime.UtcNow.Add(period)
                },

                // ---------------------------------------------------------
                // UPDATE FACTORY - This function executes when "key" already exists.
                // ---------------------------------------------------------
                // "_" represents the existing dictionary key.
                //
                // "existing" represents the existing RateLimitCounter.
                (_, existing) =>
                {
                    // Check whether the current rate-limit window has already expired.
                    // Start a completely new window.
                    if (existing.Expiry < DateTime.UtcNow)
                    {
                        existing.Count = 1;
                        existing.Expiry = DateTime.UtcNow.Add(period);
                    }
                    else
                    {
                        // The current rate-limit window is still active.
                        // Therefore, increment the number of requests.
                        existing.Count++;
                    }

                    // Return the updated counter.
                    //
                    // ConcurrentDictionary stores this returned value for the specified key.
                    return existing;
                });

            // Check whether the request is still within the allowed limit.
            // Therefore, the request that makes Count exceed the limit will be rejected.
            return counter.Count <= limit;
        }


        // Returns the number of requests that are still available for the specified key.
        // Example: WHEN limit = 10 and current Count = 3 THEN Remaining = 10 - 3 = 7
        public async Task<int> GetRemainingRequestsAsync(
            string key,
            int limit,
            CancellationToken cancellationToken = default)
        {
            // Try to find the counter associated with the key.
            if (_counters.TryGetValue(key, out var counter))
            {
                // Check whether this counter's rate-limit window has expired.
                if (counter.Expiry < DateTime.UtcNow)
                {
                    // The window has expired.
                    // From the caller's perspective, the user can make the full number of requests again.
                    // Therefore, all requests are still available so eligible for limit requests
                    return limit;
                }

                // When this counter's rate-limit window available.
                // Calculate how many requests remain.
                // Math.Max(0, ...) - prevents a negative number from being returned.
                return Math.Max(0, limit - counter.Count);
            }

            // If no counter exists for this key, then the caller hasn't made any tracked requests yet.
            // Therefore, all requests are still available so eligible for limit requests
            return limit;
        }


        // Represents the rate-limit information for a single key.
        // This class is private because it is only needed internally by InMemoryRateLimitingService.
        private class RateLimitCounter
        {
            // Number of requests made during the current rate-limit window.
            public int Count { get; set; }

            // The exact date/time when this rate-limit window expires.
            public DateTime Expiry { get; set; }
        }
    }
}
