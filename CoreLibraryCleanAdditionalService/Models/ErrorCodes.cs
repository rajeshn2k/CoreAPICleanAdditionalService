namespace Core.Library.Clean.AdditionalService
{
    /// <summary>
    /// Standardized error codes for API responses
    /// </summary>
    public static class ErrorCodes
    {
        // Validation Errors
        public const string VALIDATION_ERROR = "VALIDATION_ERROR";
        public const string INVALID_INPUT = "INVALID_INPUT";
        public const string MISSING_REQUIRED_FIELD = "MISSING_REQUIRED_FIELD";
        public const string INVALID_FORMAT = "INVALID_FORMAT";

        // Resource Not Found Errors
        public const string NOT_FOUND = "NOT_FOUND";
        public const string BOOK_NOT_FOUND = "BOOK_NOT_FOUND";
        public const string PERSON_NOT_FOUND = "PERSON_NOT_FOUND";
        public const string RESOURCE_NOT_FOUND = "RESOURCE_NOT_FOUND";

        // Authentication and Authorization Errors
        public const string UNAUTHORIZED = "UNAUTHORIZED";
        public const string FORBIDDEN = "FORBIDDEN";
        public const string INVALID_TOKEN = "INVALID_TOKEN";
        public const string TOKEN_EXPIRED = "TOKEN_EXPIRED";
        public const string INSUFFICIENT_PERMISSIONS = "INSUFFICIENT_PERMISSIONS";

        // Business Logic Errors
        public const string CONFLICT = "CONFLICT";
        public const string DUPLICATE_RESOURCE = "DUPLICATE_RESOURCE";
        public const string INVALID_OPERATION = "INVALID_OPERATION";
        public const string BUSINESS_RULE_VIOLATION = "BUSINESS_RULE_VIOLATION";

        // Rate Limiting Errors
        public const string RATE_LIMIT_EXCEEDED = "RATE_LIMIT_EXCEEDED";
        public const string TOO_MANY_REQUESTS = "TOO_MANY_REQUESTS";

        // Service Availability Errors
        public const string SERVICE_UNAVAILABLE = "SERVICE_UNAVAILABLE";
        public const string CIRCUIT_BREAKER_OPEN = "CIRCUIT_BREAKER_OPEN";
        public const string DEPENDENCY_FAILURE = "DEPENDENCY_FAILURE";

        // Database Errors
        public const string DATABASE_ERROR = "DATABASE_ERROR";
        public const string CONNECTION_ERROR = "CONNECTION_ERROR";
        public const string QUERY_ERROR = "QUERY_ERROR";

        // External Service Errors
        public const string CACHE_ERROR = "CACHE_ERROR";
        public const string MESSAGING_ERROR = "MESSAGING_ERROR";
        public const string EXTERNAL_SERVICE_ERROR = "EXTERNAL_SERVICE_ERROR";

        // General Server Errors
        public const string INTERNAL_ERROR = "INTERNAL_ERROR";
        public const string UNKNOWN_ERROR = "UNKNOWN_ERROR";
    }
}