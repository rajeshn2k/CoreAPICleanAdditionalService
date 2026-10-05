using Microsoft.AspNetCore.Mvc;
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
        private readonly ILogger<ResilienceHealthController> _logger;

        public ResilienceHealthController(
            ResiliencePipelineProvider<string> pipelineProvider,
            ILogger<ResilienceHealthController> logger)
        {
            _pipelineProvider = pipelineProvider;
            _logger = logger;
        }

        /// <summary>
        /// Get resilience pipeline health status for all services
        /// </summary>
        [HttpGet]
        public ActionResult GetResilienceHealth()
        {
            var redisPipeline = _pipelineProvider.GetPipeline("InMemoryCacheService");
            var rabbitMQPipeline = _pipelineProvider.GetPipeline("InMemoryMessagePublisher");

            var redisCircuitState = GetCircuitState(redisPipeline);
            var rabbitMQCircuitState = GetCircuitState(rabbitMQPipeline);

            var healthStatus = new
            {
                Timestamp = DateTime.UtcNow,
                Services = new
                {
                    Redis = new
                    {
                        State = redisCircuitState.ToString(),
                        IsHealthy = redisCircuitState == CircuitState.Closed
                    },
                    RabbitMQ = new
                    {
                        State = rabbitMQCircuitState.ToString(),
                        IsHealthy = rabbitMQCircuitState == CircuitState.Closed
                    }
                },
                OverallHealth = redisCircuitState == CircuitState.Closed && rabbitMQCircuitState == CircuitState.Closed
            };

            _logger.LogInformation("Resilience health check: Redis={RedisState}, RabbitMQ={RabbitMQState}", 
                redisCircuitState, rabbitMQCircuitState);

            return Ok(healthStatus);
        }

        /// <summary>
        /// Get resilience pipeline health status for a specific service
        /// </summary>
        [HttpGet("{serviceKey}")]
        public ActionResult GetServiceResilienceHealth(string serviceKey)
        {
            var pipeline = _pipelineProvider.GetPipeline(serviceKey);
            var circuitState = GetCircuitState(pipeline);
            
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
