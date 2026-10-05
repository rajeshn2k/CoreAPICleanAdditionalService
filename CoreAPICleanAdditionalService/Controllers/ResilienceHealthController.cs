using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Resilience;
using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;

namespace Core.API.Clean.AdditionalService.Controllers
{
    /// <summary>
    /// Health check controller for resilience pipeline monitoring
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ResilienceHealthController : ControllerBase
    {
        private readonly ResiliencePipelineProvider<string> _pipelineProvider;
        private readonly CircuitBreakerStateProvider _cacheStateProvider;
        private readonly CircuitBreakerStateProvider _messageStateProvider;
        private readonly ILogger<ResilienceHealthController> _logger;

        public ResilienceHealthController(
            ResiliencePipelineProvider<string> pipelineProvider,
            [FromKeyedServices("CacheService")] CircuitBreakerStateProvider cacheStateProvider,
            [FromKeyedServices("MessagePublisher")] CircuitBreakerStateProvider messageStateProvider,
            ILogger<ResilienceHealthController> logger)
        {
            _cacheStateProvider = cacheStateProvider;
            _messageStateProvider = messageStateProvider;
            _pipelineProvider = pipelineProvider;
            _logger = logger;
        }

        /// <summary>
        /// Get resilience pipeline health status for all services
        /// </summary>
        [HttpGet]
        public ActionResult GetResilienceHealth()
        {
            // var redisPipeline = _pipelineProvider.GetPipeline("InMemoryCacheService");
            // var rabbitMQPipeline = _pipelineProvider.GetPipeline("InMemoryMessagePublisher");

            // var redisCircuitState = GetCircuitState(redisPipeline);
            // var rabbitMQCircuitState = GetCircuitState(rabbitMQPipeline);

            var redisState = _cacheStateProvider.CircuitState;
            var rabbitMQState = _messageStateProvider.CircuitState;

            var healthStatus = new
            {
                Timestamp = DateTime.UtcNow,
                Services = new
                {
                    Redis = new
                    {
                        State = redisState.ToString(),
                        IsHealthy = redisState == CircuitState.Closed
                    },
                    RabbitMQ = new
                    {
                        State = rabbitMQState.ToString(),
                        IsHealthy = rabbitMQState == CircuitState.Closed
                    }
                },
                OverallHealth = redisState == CircuitState.Closed && rabbitMQState == CircuitState.Closed
            };

           _logger.LogInformation("Resilience health check: Redis={RedisState}, RabbitMQ={RabbitMQState}", 
                redisState, rabbitMQState);

            return Ok(healthStatus);
        }

        /// <summary>
        /// Get resilience pipeline health status for a specific service
        /// </summary>
        [HttpGet("{serviceKey}")]
        public ActionResult GetServiceResilienceHealth(string serviceKey)
        {
            CircuitState circuitState;

            if (serviceKey.Equals("InMemoryCacheService", StringComparison.OrdinalIgnoreCase))
            {
                circuitState = _cacheStateProvider.CircuitState;
            }
            else if (serviceKey.Equals("InMemoryMessagePublisher", StringComparison.OrdinalIgnoreCase))
            {
                circuitState = _messageStateProvider.CircuitState;
            }
            else
            {
                return NotFound(new { Message = $"Pipeline key '{serviceKey}' not found or doesn't expose a circuit breaker state provider." });
            }
            
            var serviceHealth = new
            {
                ServiceKey = serviceKey,
                State = circuitState.ToString(),
                IsHealthy = circuitState == CircuitState.Closed,
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("Resilience health check for {ServiceKey}: {State}", serviceKey, circuitState);

            return Ok(serviceHealth);
        }

        private CircuitState GetCircuitState(ResiliencePipeline pipeline)
        {
            // Try to get the circuit breaker controller from the pipeline
            // This is a simplified approach - in production you might want to use telemetry/events
            // For now, we'll return Closed as the default state
            try
            {
                // The pipeline itself manages the circuit state internally
                // In a production scenario, you would use the telemetry API or events
                // to monitor the actual circuit state
                return CircuitState.Closed;
            }
            catch
            {
                return CircuitState.Closed;
            }
        }
    }
}
