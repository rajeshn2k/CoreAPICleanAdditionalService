namespace Core.Library.Clean.AdditionalService
{
    /// <summary>
    /// Standardized API error response
    /// </summary>
    public class ApiErrorResponse
    {
        /// <summary>
        /// Indicates whether the request was successful (always false for error responses)
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Detailed error information
        /// </summary>
        public ErrorDetail Error { get; set; }

        /// <summary>
        /// Timestamp when the error response was generated
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Unique request identifier for correlation and debugging
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// The request path that caused the error
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Retry-after time in seconds (for rate limiting errors)
        /// </summary>
        public int? RetryAfter { get; set; }

        public ApiErrorResponse()
        {
            Success = false;
            Timestamp = DateTime.UtcNow;
        }

        public static ApiErrorResponse CreateError(string code, string message, int statusCode, string requestId = null, string path = null, string details = null)
        {
            return new ApiErrorResponse
            {
                Success = false,
                Error = new ErrorDetail
                {
                    Code = code,
                    Message = message,
                    Details = details,
                    StatusCode = statusCode
                },
                Timestamp = DateTime.UtcNow,
                RequestId = requestId,
                Path = path
            };
        }

        public static ApiErrorResponse CreateRateLimitError(int retryAfter, string requestId = null, string path = null)
        {
            return new ApiErrorResponse
            {
                Success = false,
                Error = new ErrorDetail
                {
                    Code = ErrorCodes.RATE_LIMIT_EXCEEDED,
                    Message = "Rate limit exceeded",
                    Details = "Maximum requests per minute exceeded. Please retry later.",
                    StatusCode = 429
                },
                Timestamp = DateTime.UtcNow,
                RequestId = requestId,
                Path = path,
                RetryAfter = retryAfter
            };
        }
    }

    /// <summary>
    /// Detailed error information
    /// </summary>
    public class ErrorDetail
    {
        /// <summary>
        /// Machine-readable error code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Human-readable error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Additional error details
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// HTTP status code
        /// </summary>
        public int StatusCode { get; set; }
    }
}