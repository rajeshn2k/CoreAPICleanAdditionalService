namespace Core.Library.Clean.AdditionalService
{
    public class ResilienceSettings
    {
        public ServiceResilienceSettings RabbitMQ { get; set; }
        public ServiceResilienceSettings Redis { get; set; }
    }

    public class ServiceResilienceSettings
    {
        public RetrySettings Retry { get; set; }
        public CircuitBreakerSettings CircuitBreaker { get; set; }
        public int TimeoutSeconds { get; set; }
    }

    public class RetrySettings
    {
        public int MaxRetryAttempts { get; set; } = 3;
        public int DelaySeconds { get; set; } = 1;
    }

    public class CircuitBreakerSettings
    {
        public double FailureRatio { get; set; } = 0.5;
        public int MinimumThroughput { get; set; } = 10;
        public int SamplingDurationSeconds { get; set; } = 30;
        public int BreakDurationSeconds { get; set; } = 30;
    }
}