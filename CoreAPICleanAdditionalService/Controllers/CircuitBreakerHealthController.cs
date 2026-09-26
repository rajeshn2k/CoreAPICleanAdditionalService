using Core.API.Clean.AdditionalService.CircuitBreaker;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Clean.AdditionalService.Controllers
{
    /// <summary>
    /// Health check controller for circuit breaker monitoring
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CircuitBreakerHealthController : ControllerBase
    {
        private readonly ICircuitBreakerService _circuitBreakerService;
        private readonly ILogger<CircuitBreakerHealthController> _logger;

        public CircuitBreakerHealthController(
            ICircuitBreakerService circuitBreakerService,
            ILogger<CircuitBreakerHealthController> logger)
        {
            _circuitBreakerService = circuitBreakerService;
            _logger = logger;
        }

        /// <summary>
        /// Get circuit breaker health status for all services
        /// </summary>
        [HttpGet]
        public ActionResult GetCircuitBreakerHealth()
        {
            var redisState = _circuitBreakerService.GetState("RedisCache");
            var rabbitMQState = _circuitBreakerService.GetState("RabbitMQ");

            var healthStatus = new
            {
                Timestamp = DateTime.UtcNow,
                Services = new
                {
                    RedisCache = new
                    {
                        State = redisState.ToString(),
                        IsHealthy = redisState == CircuitBreakerState.Closed
                    },
                    RabbitMQ = new
                    {
                        State = rabbitMQState.ToString(),
                        IsHealthy = rabbitMQState == CircuitBreakerState.Closed
                    }
                },
                OverallHealth = redisState == CircuitBreakerState.Closed && rabbitMQState == CircuitBreakerState.Closed
            };

            _logger.LogInformation("Circuit breaker health check: Redis={RedisState}, RabbitMQ={RabbitMQState}", 
                redisState, rabbitMQState);

            return Ok(healthStatus);
        }

        /// <summary>
        /// Get circuit breaker health status for a specific service
        /// </summary>
        [HttpGet("{serviceKey}")]
        public ActionResult GetServiceCircuitBreakerHealth(string serviceKey)
        {
            var state = _circuitBreakerService.GetState(serviceKey);
            
            var serviceHealth = new
            {
                ServiceKey = serviceKey,
                State = state.ToString(),
                IsHealthy = state == CircuitBreakerState.Closed,
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("Circuit breaker health check for {ServiceKey}: {State}", serviceKey, state);

            return Ok(serviceHealth);
        }
    }
}