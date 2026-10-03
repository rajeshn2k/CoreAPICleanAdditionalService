using Microsoft.Extensions.Logging;

namespace Core.Library.Clean.AdditionalService
{
    public class EmptyMessagePublisher : IMessagePublisher
    {
        private readonly ILogger<EmptyMessagePublisher> _logger;

        public EmptyMessagePublisher(ILogger<EmptyMessagePublisher> logger)
        {
            _logger = logger;
        }

        public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class
        {
            _logger.LogInformation("--> Not Connected to any Publish Messaging Queues");
            return Task.CompletedTask;
        }

        public Task PublishAsync<TMessage>(IEnumerable<TMessage> messages, CancellationToken cancellationToken = default) where TMessage : class
        {
            _logger.LogInformation("--> Not Connected to any Publish Messaging Queues");
            return Task.CompletedTask;
        }

        public Task PublishAsync(object entity, string messageType, string messageAction, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("--> Not Connected to any Publish Messaging Queues");
            return Task.CompletedTask;
        }
    }
}
