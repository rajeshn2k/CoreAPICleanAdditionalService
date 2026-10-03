/*
namespace Core.Library.Clean.AdditionalService
{
    public class InMemoryMessageConsumer : BackgroundService
    {
        private readonly InMemoryMessagePublisher _publisher;
        private readonly ILogger<InMemoryMessageConsumer> _logger;

        public InMemoryMessageConsumer(
            InMemoryMessagePublisher publisher,
            ILogger<InMemoryMessageConsumer> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "In-memory message consumer started");

            await foreach (var message in
                _publisher.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await ProcessMessageAsync(
                        message,
                        stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error processing in-memory message " +
                        "{RoutingKey}",
                        message.RoutingKey);
                }
            }
        }

        private async Task ProcessMessageAsync(
            InMemoryMessage message,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Processing message: {MessageType}, " +
                "RoutingKey: {RoutingKey}, " +
                "CorrelationId: {CorrelationId}",
                message.MessageType,
                message.RoutingKey,
                message.CorrelationId);

            switch (message.Message)
            {
                case BookCreatedMessage bookCreated:
                    await ProcessBookCreated(
                        bookCreated,
                        cancellationToken);
                    break;

                case BookUpdatedMessage bookUpdated:
                    await ProcessBookUpdated(
                        bookUpdated,
                        cancellationToken);
                    break;

                case PersonCreatedMessage personCreated:
                    await ProcessPersonCreated(
                        personCreated,
                        cancellationToken);
                    break;

                case PersonUpdatedMessage personUpdated:
                    await ProcessPersonUpdated(
                        personUpdated,
                        cancellationToken);
                    break;

                default:
                    _logger.LogWarning(
                        "Unknown message type: {MessageType}",
                        message.MessageType);
                    break;
            }
        }

        private Task ProcessBookCreated(
            BookCreatedMessage message,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Book created: {BookId}",
                message.BookId);

            // Your actual processing logic here.

            return Task.CompletedTask;
        }

        private Task ProcessBookUpdated(
            BookUpdatedMessage message,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Book updated: {BookId}",
                message.BookId);

            // Your actual processing logic here.

            return Task.CompletedTask;
        }

        private Task ProcessPersonCreated(
            PersonCreatedMessage message,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Person created: {PersonId}",
                message.PersonId);

            // Your actual processing logic here.

            return Task.CompletedTask;
        }

        private Task ProcessPersonUpdated(
            PersonUpdatedMessage message,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Person updated: {PersonId}",
                message.PersonId);

            // Your actual processing logic here.

            return Task.CompletedTask;
        }
    }
}
*/