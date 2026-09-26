namespace Core.Library.Clean.AdditionalService
{
    public class CircuitBreakerSettings
    {
        public RedisCircuitBreakerSettings Redis { get; set; }
        public RabbitMQCircuitBreakerSettings RabbitMQ { get; set; }
    }

    public class RedisCircuitBreakerSettings
    {
        public int ExceptionsAllowedBeforeBreaking { get; set; } = 5;
        public int DurationOfBreakInSeconds { get; set; } = 30;
        public int RetryCount { get; set; } = 3;
        public int RetryDelayInSeconds { get; set; } = 1;
        public int TimeoutInSeconds { get; set; } = 5;
    }

    public class RabbitMQCircuitBreakerSettings
    {
        public int ExceptionsAllowedBeforeBreaking { get; set; } = 3;
        public int DurationOfBreakInSeconds { get; set; } = 60;
        public int RetryCount { get; set; } = 5;
        public int RetryDelayInSeconds { get; set; } = 2;
        public int TimeoutInSeconds { get; set; } = 10;
    }
}