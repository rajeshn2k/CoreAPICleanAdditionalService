using Core.Library.Clean.AdditionalService;
using Core.Library.Clean.AdditionalService.Messaging.Contracts;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;

namespace Core.API.Clean.AdditionalService.CircuitBreaker
{
    public class CircuitBreakerMessagePublisher : IMessagePublisher
    {
        private readonly IMessagePublisher _innerMessagePublisher;
        private readonly ICircuitBreakerService _circuitBreakerService;
        private readonly ILogger<CircuitBreakerMessagePublisher> _logger;
        private const string ServiceKey = "RabbitMQ";

        public CircuitBreakerMessagePublisher(
            IMessagePublisher innerMessagePublisher,
            ICircuitBreakerService circuitBreakerService,
            ILogger<CircuitBreakerMessagePublisher> logger)
        {
            _innerMessagePublisher = innerMessagePublisher;
            _circuitBreakerService = circuitBreakerService;
            _logger = logger;
        }

        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class
        {
            try
            {
                await _circuitBreakerService.ExecuteAsync(ServiceKey, async () =>
                {
                    await _innerMessagePublisher.PublishAsync(message, cancellationToken);
                }, cancellationToken);
            }
            catch (BrokenCircuitException)
            {
                _logger.LogWarning("Circuit breaker is open for RabbitMQ, skipping message publish for message type: {MessageType}", 
                    message?.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in message publisher PublishAsync for message type: {MessageType}", 
                    message?.GetType().Name);
            }
        }

        public async Task PublishAsync<TMessage>(IEnumerable<TMessage> messages, CancellationToken cancellationToken = default) where TMessage : class
        {
            try
            {
                await _circuitBreakerService.ExecuteAsync(ServiceKey, async () =>
                {
                    await _innerMessagePublisher.PublishAsync(messages, cancellationToken);
                }, cancellationToken);
            }
            catch (BrokenCircuitException)
            {
                _logger.LogWarning("Circuit breaker is open for RabbitMQ, skipping batch message publish for {MessageCount} messages", 
                    messages?.Count() ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in message publisher PublishAsync for batch of {MessageCount} messages", 
                    messages?.Count() ?? 0);
            }
        }

        public async Task PublishAsync(object entity, string messageType, string messageAction, CancellationToken cancellationToken = default)
        {
            try
            {
                await _circuitBreakerService.ExecuteAsync(ServiceKey, async () =>
                {
                    await _innerMessagePublisher.PublishAsync(entity, messageType, messageAction, cancellationToken);
                }, cancellationToken);
            }
            catch (BrokenCircuitException)
            {
                _logger.LogWarning("Circuit breaker is open for RabbitMQ, skipping message publish for type: {MessageType}, action: {MessageAction}", 
                    messageType, messageAction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in message publisher PublishAsync for type: {MessageType}, action: {MessageAction}", 
                    messageType, messageAction);
            }
        }
    }
}