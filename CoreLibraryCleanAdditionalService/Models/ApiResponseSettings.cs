namespace Core.Library.Clean.AdditionalService
{
    /// <summary>
    /// Configuration settings for API response behavior
    /// </summary>
    public class ApiResponseSettings
    {
        /// <summary>
        /// Whether to include timestamp in all API responses
        /// </summary>
        public bool IncludeTimestamp { get; set; } = true;

        /// <summary>
        /// Whether to include request ID in all API responses
        /// </summary>
        public bool IncludeRequestId { get; set; } = true;

        /// <summary>
        /// Whether to include detailed error information in error responses
        /// </summary>
        public bool DetailedErrors { get; set; } = true;
    }
}