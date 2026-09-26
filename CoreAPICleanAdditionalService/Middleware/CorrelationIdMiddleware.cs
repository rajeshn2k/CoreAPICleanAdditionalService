using Core.Library.Clean.AdditionalService;

namespace Core.API.Clean.AdditionalService.Middleware
{
    /// <summary>
    /// Middleware to handle correlation ID for request tracking and debugging
    /// </summary>
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if correlation ID is already present in headers
            var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();
            
            // If not present, generate a new one
            if (string.IsNullOrEmpty(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            // Store correlation ID in HttpContext items for later use
            context.Items["CorrelationId"] = correlationId;

            // Add correlation ID to response headers
            context.Response.Headers["X-Correlation-ID"] = correlationId;

            // Log the correlation ID for tracking
            _logger.LogDebug("Processing request with CorrelationId: {CorrelationId}", correlationId);

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request with CorrelationId: {CorrelationId}", correlationId);
                throw;
            }
        }
    }

    /// <summary>
    /// Extension methods for registering CorrelationIdMiddleware
    /// </summary>
    public static class CorrelationIdMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorrelationIdMiddleware>();
        }
    }
}