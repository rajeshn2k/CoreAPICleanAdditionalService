using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Core.Library.Clean.AdditionalService;

namespace Core.API.Clean.AdditionalService.RateLimiting
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private readonly RateLimitingSettings _settings;
        private readonly IRateLimitingService _rateLimitingService;

        public RateLimitingMiddleware(
            RequestDelegate next,
            ILogger<RateLimitingMiddleware> logger,
            IOptions<RateLimitingSettings> settings,
            IRateLimitingService rateLimitingService)
        {
            _next = next;
            _logger = logger;
            _settings = settings.Value;
            _rateLimitingService = rateLimitingService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_settings.EnableRateLimiting)
            {
                await _next(context);
                return;
            }

            // Determine user role and appropriate rate limit
            var userRole = GetUserRole(context);
            var rateLimit = GetRateLimit(userRole);
            var clientIdentifier = GetClientIdentifier(context);

            // Check if request is allowed
            var isAllowed = await _rateLimitingService.IsAllowedAsync(
                clientIdentifier,
                rateLimit,
                TimeSpan.FromMinutes(1));

            if (!isAllowed)
            {
                _logger.LogWarning("Rate limit exceeded for {ClientIdentifier} with role {UserRole}. Limit: {Limit}", 
                    clientIdentifier, userRole, rateLimit);

                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.Headers.Append("X-RateLimit-Limit", rateLimit.ToString());
                context.Response.Headers.Append("X-RateLimit-Remaining", "0");
                context.Response.Headers.Append("Retry-After", "60");
                
                var errorResponse = new
                {
                    success = false,
                    error = new
                    {
                        code = "RATE_LIMIT_EXCEEDED",
                        message = $"Rate limit exceeded. Maximum {rateLimit} requests per minute allowed.",
                        statusCode = 429
                    },
                    timestamp = DateTime.UtcNow,
                    retryAfter = 60
                };

                await context.Response.WriteAsJsonAsync(errorResponse);
                return;
            }

            // Add rate limit headers
            var remaining = await _rateLimitingService.GetRemainingRequestsAsync(clientIdentifier, rateLimit);
            context.Response.Headers.Append("X-RateLimit-Limit", rateLimit.ToString());
            context.Response.Headers.Append("X-RateLimit-Remaining", remaining.ToString());

            await _next(context);
        }

        private string GetUserRole(HttpContext context)
        {
            // Check if user is authenticated
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                // Check for admin role
                if (context.User.IsInRole("Admin"))
                {
                    return "Admin";
                }
                return "Authenticated";
            }

            return "Anonymous";
        }

        private string GetClientIdentifier(HttpContext context)
        {
            // Try to get user ID from claims if authenticated
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst("sub")?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    return $"user:{userId}";
                }
            }

            // Fall back to IP address
            return $"ip:{context.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";
        }

        private int GetRateLimit(string userRole)
        {
            return userRole switch
            {
                "Admin" => _settings?.GeneralRules?.Admin?.PerMinute ?? 5000,
                "Authenticated" => _settings?.GeneralRules?.Authenticated?.PerMinute ?? 1000,
                _ => _settings?.GeneralRules?.Anonymous?.PerMinute ?? 100
            };
        }
    }
}