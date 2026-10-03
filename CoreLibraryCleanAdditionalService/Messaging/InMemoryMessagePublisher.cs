using Core.Library.Clean.AdditionalService.ResilientTest;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Core.Library.Clean.AdditionalService
{
    /// <summary>
    /// In-memory implementation of IMessagePublisher.
    /// </summary>
    public class InMemoryMessagePublisher : IMessagePublisher, IDisposable
    {
        private readonly ILogger<InMemoryMessagePublisher> _logger;
        private readonly ResilientTestService resilientTestService;
        private readonly Channel<InMemoryMessage> _channel;
        //e-exception, t-timeout, a-default
        private readonly string testMode = "a";

        public InMemoryMessagePublisher(ILogger<InMemoryMessagePublisher> logger, ResilientTestService resilientTestService)
        {
            _logger = logger;
            this.resilientTestService = resilientTestService;
            _channel = Channel.CreateUnbounded<InMemoryMessage>(
                new UnboundedChannelOptions
                {
                    SingleReader = false,
                    SingleWriter = false,
                    AllowSynchronousContinuations = false
                });

            _logger.LogInformation("In-memory message publisher initialized successfully");
        }

        public async Task PublishAsync<TMessage>(TMessage message,
            CancellationToken cancellationToken) where TMessage : class
        {
            ArgumentNullException.ThrowIfNull(message);

            //e-exception, t-timeout, a-default
            await resilientTestService.InjectIssueDelayExceptionNone(testMode, cancellationToken);

            var routingKey = GetMessageRoutingKey(message);

            var inMemoryMessage = new InMemoryMessage
            {
                Message = message,
                MessageType = message.GetType().Name,
                RoutingKey = routingKey,
                Timestamp = DateTime.UtcNow,
                CorrelationId = Guid.NewGuid().ToString()
            };

            /* Having 'NO' exception handler here will let Resilience Pipelin to enable the statergy
              * Polly Resilience Pipelin Implements staergy or policies such as Retry, Circuit breaker, TimeOut
              * ResilientMessagePublisher is the caller or excuter for actual InMemoryMessagePublisher*/

            await _channel.Writer.WriteAsync(inMemoryMessage, cancellationToken);

            _logger.LogDebug("Message published to in-memory channel. MessageType: {MessageType}, RoutingKey: {RoutingKey}",
                inMemoryMessage.MessageType, routingKey);
        }

        public async Task PublishAsync<TMessage>(IEnumerable<TMessage> messages,
            CancellationToken cancellationToken)
            where TMessage : class
        {
            if (messages == null)
            {
                return;
            }

            foreach (var message in messages)
            {
                await PublishAsync(message, cancellationToken);
            }
        }

        public async Task PublishAsync(
            object entity,
            string messageType,
            string messageAction,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var message = CreateMessage(entity, messageType, messageAction);

            await PublishAsync(message, cancellationToken);
        }

        /// <summary>
        /// Allows a background service/test to consume published messages.
        /// </summary>
        public IAsyncEnumerable<InMemoryMessage> ReadAllAsync(
            CancellationToken cancellationToken = default)
        {
            return _channel.Reader.ReadAllAsync(cancellationToken);
        }

        /// <summary>
        /// Attempts to read a message immediately without waiting.
        /// Useful in unit/integration tests.
        /// </summary>
        public bool TryRead(out InMemoryMessage? message)
        {
            return _channel.Reader.TryRead(out message);
        }

        private string GetMessageRoutingKey<TMessage>(TMessage message)
            where TMessage : class
        {
            return message switch
            {
                BookCreatedMessage => "book.created",
                BookUpdatedMessage => "book.updated",
                BookDeletedMessage => "book.deleted",

                PersonCreatedMessage => "person.created",
                PersonUpdatedMessage => "person.updated",
                PersonDeletedMessage => "person.deleted",

                _ => "default"
            };
        }

        private IMessage CreateMessage(
            object entity,
            string messageType,
            string messageAction)
        {
            var correlationId = Guid.NewGuid().ToString();

            if (messageType == MessageTypeConstant.BookType && messageAction == MessageActionConstant.Create)
            {
                return new BookCreatedMessage
                {
                    BookId = GetEntityId(entity),
                    BookData = entity as BookDTO,
                    Timestamp = DateTime.UtcNow,
                    CorrelationId = correlationId
                };
            }

            if (messageType == MessageTypeConstant.BookType && messageAction == MessageActionConstant.Update)
            {
                return new BookUpdatedMessage
                {
                    BookId = GetEntityId(entity),
                    BookData = entity as BookDTO,
                    Timestamp = DateTime.UtcNow,
                    CorrelationId = correlationId
                };
            }

            if (messageType == MessageTypeConstant.PersonType && messageAction == MessageActionConstant.Create)
            {
                return new PersonCreatedMessage
                {
                    PersonId = GetEntityId(entity),
                    PersonData = entity as PersonDTO,
                    Timestamp = DateTime.UtcNow,
                    CorrelationId = correlationId
                };
            }

            if (messageType == MessageTypeConstant.PersonType && messageAction == MessageActionConstant.Update)
            {
                return new PersonUpdatedMessage
                {
                    PersonId = GetEntityId(entity),
                    PersonData = entity as PersonDTO,
                    Timestamp = DateTime.UtcNow,
                    CorrelationId = correlationId
                };
            }

            throw new ArgumentException($"Unknown message type: {messageType}");
        }

        private string GetEntityId(object entity)
        {
            return entity switch
            {
                BookDTO book => book.id,
                PersonDTO person => person.id,
                Book book => book.Id,
                Person person => person.Id,
                _ => string.Empty
            };
        }

        public void Dispose()
        {
            _channel.Writer.TryComplete();

            _logger.LogInformation("In-memory message publisher disposed");
        }
    }

    /// <summary>
    /// Represents a message stored in the in-memory message channel.
    /// </summary>
    public sealed class InMemoryMessage
    {
        public required object Message { get; init; }

        public required string MessageType { get; init; }

        public required string RoutingKey { get; init; }

        public required string CorrelationId { get; init; }

        public DateTime Timestamp { get; init; }
    }
}
