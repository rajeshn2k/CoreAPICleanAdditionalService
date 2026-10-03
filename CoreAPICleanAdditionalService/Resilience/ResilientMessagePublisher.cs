using Core.Library.Clean.AdditionalService;
using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using Polly.Timeout;

namespace Core.API.Clean.AdditionalService.Resilience
{
    /// <summary>
    /// Resilient decorator for IMessagePublisher using Microsoft.Extensions.Resilience
    /// </summary>
    public sealed class ResilientMessagePublisher : IMessagePublisher
    {
        private const string PipelineKey = "InMemoryMessagePublisher";
        private readonly IMessagePublisher _innerMessagePublisher;
        private readonly ResiliencePipeline _pipeline;
        private readonly ILogger<ResilientMessagePublisher> _logger;

        public ResilientMessagePublisher(
            IMessagePublisher innerMessagePublisher,
            ResiliencePipelineProvider<string> pipelineProvider,
            ILogger<ResilientMessagePublisher> logger)
        {
            _innerMessagePublisher = innerMessagePublisher;
            _pipeline = pipelineProvider.GetPipeline(PipelineKey);
            _logger = logger;
        }

        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : class
        {
            try
            {
                await _pipeline.ExecuteAsync(
                    async token =>
                    {
                        await _innerMessagePublisher.PublishAsync(message, token);
                    },
                    cancellationToken);
            }
            /*
             * ONLY handle Exception produced by Polly Resilience Pipelin Implements staergy
             * policies such as Retry, Circuit breaker, TimeOut (Expected Behaviour)
             * 1.TimeoutRejectedException,2.BrokenCircuitException
             * Resilience - application's ability to withstand and rapidly recover from disruptions, failures
             * Expectation is failure due to InMemoryMessagePublisher should not break API data access
             */
            catch (TimeoutRejectedException ex)
            {
                //_logger.LogWarning(ex, "ResilientMessagePublisher operation timed out for  MessageType: {MessageType}", message?.GetType().Name);
                _logger.LogWarning("InMemoryMessagePublisher operation timed out for  MessageType: {MessageType}", message?.GetType().Name);
            }
            catch (BrokenCircuitException ex)
            {
                // _logger.LogWarning(ex, "InMemoryMessagePublisher circuit breaker is open. Message was not published. MessageType: {MessageType}", message?.GetType().Name);
                _logger.LogWarning("ResilientMessagePublisher circuit breaker is open. Message was not published. MessageType: {MessageType}", message?.GetType().Name);
            }
            /*
             * it will catch essentially any exception that escapes _pipeline.ExecuteAsync(...) and turn it into a PUBLISH MISS 
             * So if _innerMessagePublisher.PublishAsync<T>() throws something unexpected, such as:
             * 1.RedisConnectionException 2.RedisTimeoutException 3.NullReferenceException 4.ArgumentException 5.InvalidOperationException 6.OperationCanceledException 7.OutOfMemoryException
             * IF YOU WANT CancellationToken PROPAGATE BACK TO DIRECTOR, THIS WILL PREVENT
             */
            catch (Exception ex)
            {
                //_logger.LogWarning(ex, "UN-HANDLED EXCEPTION swallowing programming bugs and cancellation: {Key}", key);
                _logger.LogWarning("UN-HANDLED EXCEPTION swallowing programming bugs and cancellation");
            }
        }

        public async Task PublishAsync<TMessage>(IEnumerable<TMessage> messages, CancellationToken cancellationToken = default) where TMessage : class
        {
            try
            {
                await _pipeline.ExecuteAsync(
                    async token =>
                    {
                        await _innerMessagePublisher.PublishAsync(messages, token);
                    },
                    cancellationToken);
            }
            /*
             * ONLY handle Exception produced by Polly Resilience Pipelin Implements staergy
             * policies such as Retry, Circuit breaker, TimeOut (Expected Behaviour)
             * 1.TimeoutRejectedException,2.BrokenCircuitException
             * Resilience - application's ability to withstand and rapidly recover from disruptions, failures
             * Expectation is failure due to InMemoryMessagePublisher should not break API data access
             */
            catch (TimeoutRejectedException ex)
            {
                //_logger.LogWarning(ex, "ResilientMessagePublisher operation timed out for  MessageType: {MessageType}", message?.GetType().Name);
                _logger.LogWarning("InMemoryMessagePublisher operation timed out");
            }
            catch (BrokenCircuitException ex)
            {
                // _logger.LogWarning(ex, "InMemoryMessagePublisher circuit breaker is open. Message was not published. MessageType: {MessageType}", message?.GetType().Name);
                _logger.LogWarning("ResilientMessagePublisher circuit breaker is open. Message was not published");
            }
            /*
             * it will catch essentially any exception that escapes _pipeline.ExecuteAsync(...) and turn it into a PUBLISH MISS 
             * So if _innerMessagePublisher.PublishAsync<T>() throws something unexpected, such as:
             * 1.RedisConnectionException 2.RedisTimeoutException 3.NullReferenceException 4.ArgumentException 5.InvalidOperationException 6.OperationCanceledException 7.OutOfMemoryException
             * IF YOU WANT CancellationToken PROPAGATE BACK TO DIRECTOR, THIS WILL PREVENT
             */
            catch (Exception ex)
            {
                //_logger.LogWarning(ex, "UN-HANDLED EXCEPTION swallowing programming bugs and cancellation: {Key}", key);
                _logger.LogWarning("UN-HANDLED EXCEPTION swallowing programming bugs and cancellation");
            }
        }

        public async Task PublishAsync(object entity, string messageType, string messageAction, CancellationToken cancellationToken = default)
        {
            try
            {
                await _pipeline.ExecuteAsync(
                    async token =>
                    {
                        await _innerMessagePublisher.PublishAsync(entity, messageType, messageAction, token);
                    },
                    cancellationToken);
            }
            /*
              * ONLY handle Exception produced by Polly Resilience Pipelin Implements staergy
              * policies such as Retry, Circuit breaker, TimeOut (Expected Behaviour)
              * 1.TimeoutRejectedException,2.BrokenCircuitException
              * Resilience - application's ability to withstand and rapidly recover from disruptions, failures
              * Expectation is failure due to InMemoryMessagePublisher should not break API data access
              */
            catch (TimeoutRejectedException ex)
            {
                //_logger.LogWarning(ex, "ResilientMessagePublisher operation timed out for  MessageType: {MessageType}", message?.GetType().Name);
                _logger.LogWarning("InMemoryMessagePublisher operation timed out for  MessageType: {MessageType}", messageType);
            }
            catch (BrokenCircuitException ex)
            {
                // _logger.LogWarning(ex, "InMemoryMessagePublisher circuit breaker is open. Message was not published. MessageType: {MessageType}", message?.GetType().Name);
                _logger.LogWarning("ResilientMessagePublisher circuit breaker is open. Message was not published. MessageType: {MessageType}", messageType);
            }
            /*
             * it will catch essentially any exception that escapes _pipeline.ExecuteAsync(...) and turn it into a PUBLISH MISS 
             * So if _innerMessagePublisher.PublishAsync<T>() throws something unexpected, such as:
             * 1.RedisConnectionException 2.RedisTimeoutException 3.NullReferenceException 4.ArgumentException 5.InvalidOperationException 6.OperationCanceledException 7.OutOfMemoryException
             * IF YOU WANT CancellationToken PROPAGATE BACK TO DIRECTOR, THIS WILL PREVENT
             */
            catch (Exception ex)
            {
                //_logger.LogWarning(ex, "UN-HANDLED EXCEPTION swallowing programming bugs and cancellation: {Key}", key);
                _logger.LogWarning("UN-HANDLED EXCEPTION swallowing programming bugs and cancellation");
            }
        }
    }
}
