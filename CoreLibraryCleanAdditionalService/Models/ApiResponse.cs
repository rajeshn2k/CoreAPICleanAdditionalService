namespace Core.Library.Clean.AdditionalService
{
    /// <summary>
    /// Standardized API response wrapper for successful responses
    /// </summary>
    /// <typeparam name="T">Type of data being returned</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates whether the request was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The response data
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Optional message describing the result
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Timestamp when the response was generated
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Unique request identifier for correlation and debugging
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Pagination metadata for list responses
        /// </summary>
        public PaginationMetadata Pagination { get; set; }

        public ApiResponse()
        {
            Timestamp = DateTime.UtcNow;
        }

        public static ApiResponse<T> CreateSuccess(T data, string message = null, string requestId = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message ?? "Operation completed successfully",
                Timestamp = DateTime.UtcNow,
                RequestId = requestId
            };
        }

        public static ApiResponse<T> CreateSuccessWithPagination(T data, PaginationMetadata pagination, string message = null, string requestId = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message ?? "Operation completed successfully",
                Timestamp = DateTime.UtcNow,
                RequestId = requestId,
                Pagination = pagination
            };
        }
    }
}