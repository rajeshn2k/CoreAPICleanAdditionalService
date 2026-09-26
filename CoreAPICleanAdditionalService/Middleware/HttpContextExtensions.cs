namespace Core.API.Clean.AdditionalService.Middleware
{
    /// <summary>
    /// Extension methods for HttpContext to access correlation ID
    /// </summary>
    public static class HttpContextExtensions
    {
        private const string CorrelationIdKey = "CorrelationId";

        /// <summary>
        /// Gets the correlation ID from the current HttpContext
        /// </summary>
        public static string GetCorrelationId(this HttpContext context)
        {
            if (context == null)
            {
                return null;
            }

            if (context.Items.TryGetValue(CorrelationIdKey, out var correlationId))
            {
                return correlationId?.ToString();
            }

            return null;
        }

        /// <summary>
        /// Sets the correlation ID in the current HttpContext
        /// </summary>
        public static void SetCorrelationId(this HttpContext context, string correlationId)
        {
            if (context != null)
            {
                context.Items[CorrelationIdKey] = correlationId;
            }
        }
    }
}