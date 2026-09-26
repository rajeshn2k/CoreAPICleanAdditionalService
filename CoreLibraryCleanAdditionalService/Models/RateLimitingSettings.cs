namespace Core.Library.Clean.AdditionalService
{
    public class RateLimitingSettings
    {
        public bool EnableRateLimiting { get; set; }
        public bool UseDistributedRateLimiting { get; set; }
        public StackExchangeRedisOptions StackExchangeRedisOptions { get; set; }
        public GeneralRateLimitRules GeneralRules { get; set; }
        public EndpointRateLimitRules EndpointRules { get; set; }
    }

    public class StackExchangeRedisOptions
    {
        public string ConnectionMultiplexer { get; set; }
    }

    public class GeneralRateLimitRules
    {
        public RateLimitRule Anonymous { get; set; }
        public RateLimitRule Authenticated { get; set; }
        public RateLimitRule Admin { get; set; }
    }

    public class RateLimitRule
    {
        public int PerMinute { get; set; }
        public int PerHour { get; set; }
    }

    public class EndpointRateLimitRules
    {
        public EndpointMultiplierRule Read { get; set; }
        public EndpointMultiplierRule Write { get; set; }
        public EndpointMultiplierRule Delete { get; set; }
    }

    public class EndpointMultiplierRule
    {
        public double Multiplier { get; set; }
    }
}