using Core.Library.Clean.AdditionalService;
using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;

namespace Core.API.Clean.AdditionalService.Resilience
{
    /// <summary>
    /// Resilient decorator for IMessagePublisher using Microsoft.Extensions.Resilience
    /// </summary>
    public sealed class ResilientMessagePublisher : IMessagePublisher
    {
        private const string PipelineKey = "RabbitMQ";
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

        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class
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
            catch (BrokenCircuitException ex)
            {
                _logger.LogWarning(ex, "RabbitMQ circuit breaker is open. Message was not published. MessageType: {MessageType}", 
                    message?.GetType().Name);
                throw;
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
            catch (BrokenCircuitException ex)
            {
                _logger.LogWarning(ex, "RabbitMQ circuit breaker is open. Batch message publish failed. MessageCount: {MessageCount}", 
                    messages?.Count() ?? 0);
                throw;
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
            catch (BrokenCircuitException ex)
            {
                _logger.LogWarning(ex, "RabbitMQ circuit breaker is open. Message publish failed. MessageType: {MessageType}, MessageAction: {MessageAction}", 
                    messageType, messageAction);
                throw;
            }
        }
    }
}
