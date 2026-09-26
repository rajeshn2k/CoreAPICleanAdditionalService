using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Core.Library.Clean.AdditionalService;

namespace Core.API.Clean.AdditionalService.CircuitBreaker
{
    public interface ICircuitBreakerService
    {
        Task<T> ExecuteAsync<T>(string serviceKey, Func<Task<T>> action, CancellationToken cancellationToken = default);
        Task ExecuteAsync(string serviceKey, Func<Task> action, CancellationToken cancellationToken = default);
        CircuitBreakerState GetState(string serviceKey);
    }

    public enum CircuitBreakerState
    {
        Closed,
        Open,
        HalfOpen
    }

    public class CircuitBreakerService : ICircuitBreakerService
    {
        private readonly ILogger<CircuitBreakerService> _logger;
        private readonly Dictionary<string, IAsyncPolicy> _policies;
        private readonly Dictionary<string, CircuitBreakerState> _states;

        public CircuitBreakerService(ILogger<CircuitBreakerService> logger)
        {
            _logger = logger;
            _policies = new Dictionary<string, IAsyncPolicy>();
            _states = new Dictionary<string, CircuitBreakerState>();
        }

        public void AddPolicy(string serviceKey, IAsyncPolicy policy)
        {
            _policies[serviceKey] = policy;
            _states[serviceKey] = CircuitBreakerState.Closed;
            _logger.LogInformation("Added circuit breaker policy for service: {ServiceKey}", serviceKey);
        }

        public async Task<T> ExecuteAsync<T>(string serviceKey, Func<Task<T>> action, CancellationToken cancellationToken = default)
        {
            if (!_policies.ContainsKey(serviceKey))
            {
                _logger.LogWarning("No circuit breaker policy found for service: {ServiceKey}, executing without circuit breaker", serviceKey);
                return await action();
            }

            try
            {
                var result = await _policies[serviceKey].ExecuteAsync(async () => await action());
                _states[serviceKey] = CircuitBreakerState.Closed;
                return result;
            }
            catch (BrokenCircuitException)
            {
                _states[serviceKey] = CircuitBreakerState.Open;
                _logger.LogWarning("Circuit breaker is OPEN for service: {ServiceKey}", serviceKey);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing action for service: {ServiceKey}", serviceKey);
                throw;
            }
        }

        public async Task ExecuteAsync(string serviceKey, Func<Task> action, CancellationToken cancellationToken = default)
        {
            if (!_policies.ContainsKey(serviceKey))
            {
                _logger.LogWarning("No circuit breaker policy found for service: {ServiceKey}, executing without circuit breaker", serviceKey);
                await action();
                return;
            }

            try
            {
                await _policies[serviceKey].ExecuteAsync(async () => await action());
                _states[serviceKey] = CircuitBreakerState.Closed;
            }
            catch (BrokenCircuitException)
            {
                _states[serviceKey] = CircuitBreakerState.Open;
                _logger.LogWarning("Circuit breaker is OPEN for service: {ServiceKey}", serviceKey);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing action for service: {ServiceKey}", serviceKey);
                throw;
            }
        }

        public CircuitBreakerState GetState(string serviceKey)
        {
            return _states.ContainsKey(serviceKey) ? _states[serviceKey] : CircuitBreakerState.Closed;
        }
    }

    public static class CircuitBreakerPolicyFactory
    {
        public static IAsyncPolicy CreateCircuitBreakerPolicy(
            string serviceKey,
            int exceptionsAllowedBeforeBreaking,
            TimeSpan durationOfBreak,
            ILogger logger)
        {
            return Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: exceptionsAllowedBeforeBreaking,
                    durationOfBreak: durationOfBreak,
                    onBreak: (exception, breakDelay) =>
                    {
                        logger.LogWarning("Circuit breaker OPEN for {ServiceKey} after {ExceptionsAllowed} exceptions. Duration: {Duration}s",
                            serviceKey, exceptionsAllowedBeforeBreaking, breakDelay.TotalSeconds);
                    },
                    onReset: () =>
                    {
                        logger.LogInformation("Circuit breaker RESET for {ServiceKey}", serviceKey);
                    },
                    onHalfOpen: () =>
                    {
                        logger.LogInformation("Circuit breaker HALF-OPEN for {ServiceKey}", serviceKey);
                    });
        }

        public static IAsyncPolicy CreateRetryPolicy(
            string serviceKey,
            int retryCount,
            TimeSpan retryDelay,
            ILogger logger)
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: retryCount,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) * retryDelay.TotalSeconds),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        logger.LogWarning("Retry {RetryCount}/{MaxRetries} for {ServiceKey} after {Delay}s due to: {Exception}",
                            retryCount, retryCount, serviceKey, timeSpan.TotalSeconds, exception.Message);
                    });
        }

        public static IAsyncPolicy CreateTimeoutPolicy(
            string serviceKey,
            TimeSpan timeout,
            ILogger logger)
        {
            return Policy
                .TimeoutAsync(
                    timeout: timeout,
                    timeoutStrategy: Polly.Timeout.TimeoutStrategy.Optimistic,
                    onTimeout: (context, timeSpan, task) =>
                    {
                        logger.LogWarning("Timeout occurred for {ServiceKey} after {Timeout}s", serviceKey, timeSpan.TotalSeconds);
                    });
        }
    }
}