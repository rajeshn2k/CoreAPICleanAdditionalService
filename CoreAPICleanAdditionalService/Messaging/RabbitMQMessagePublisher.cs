using Core.Library.Clean.AdditionalService;
using Core.Library.Clean.AdditionalService.Messaging.Contracts;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Core.API.Clean.AdditionalService.Messaging
{
    /// <summary>
    /// RabbitMQ implementation of message publisher
    /// </summary>
    public class RabbitMQMessagePublisher : IMessagePublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQMessagePublisher> _logger;
        private readonly MessagingSettings _settings;
        private readonly string _exchangeName;

        public RabbitMQMessagePublisher(MessagingSettings settings, ILogger<RabbitMQMessagePublisher> logger)
        {
            _settings = settings;
            _logger = logger;
            _exchangeName = _settings.ExchangeName;

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _settings.ConnectionString.Replace("amqp://", "").Split(':')[0],
                    Port = int.Parse(_settings.ConnectionString.Split(':')[2].Split('/')[0]),
                    UserName = "guest",
                    Password = "guest"
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Declare exchange
                _channel.ExchangeDeclare(_exchangeName, _settings.ExchangeType, durable: true);

                _logger.LogInformation("RabbitMQ message publisher initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize RabbitMQ message publisher");
                throw;
            }
        }

        public RabbitMQMessagePublisher(IOptions<MessagingSettings> settings, ILogger<RabbitMQMessagePublisher> logger)
        {
            _settings = settings.Value;
            _logger = logger;
            _exchangeName = _settings.ExchangeName;

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _settings.ConnectionString.Replace("amqp://", "").Split(':')[0],
                    Port = int.Parse(_settings.ConnectionString.Split(':')[2].Split('/')[0]),
                    UserName = "guest",
                    Password = "guest"
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Declare exchange
                _channel.ExchangeDeclare(_exchangeName, _settings.ExchangeType, durable: true);

                _logger.LogInformation("RabbitMQ message publisher initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize RabbitMQ message publisher");
                throw;
            }
        }

        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class
        {
            try
            {
                if (!_settings.EnableMessaging)
                {
                    _logger.LogDebug("Messaging is disabled, skipping publish");
                    return;
                }

                var messageBody = JsonConvert.SerializeObject(message);
                var body = System.Text.Encoding.UTF8.GetBytes(messageBody);

                var routingKey = GetMessageRoutingKey(message);

                _channel.BasicPublish(
                    exchange: _exchangeName,
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);

                _logger.LogDebug("Message published to exchange {Exchange} with routing key {RoutingKey}", _exchangeName, routingKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message");
                throw;
            }
        }

        public async Task PublishAsync<TMessage>(IEnumerable<TMessage> messages, CancellationToken cancellationToken = default) where TMessage : class
        {
            if (messages == null || !messages.Any())
            {
                return;
            }

            foreach (var message in messages)
            {
                await PublishAsync(message, cancellationToken);
            }
        }

        public async Task PublishAsync(object entity, string messageType, string messageAction, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!_settings.EnableMessaging)
                {
                    _logger.LogDebug("Messaging is disabled, skipping publish");
                    return;
                }

                var message = CreateMessage(entity, messageType, messageAction);
                await PublishAsync(message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message for entity type {MessageType}", messageType);
                throw;
            }
        }

        private string GetMessageRoutingKey<TMessage>(TMessage message) where TMessage : class
        {
            if (message is BookCreatedMessage)
                return "book.created";
            if (message is BookUpdatedMessage)
                return "book.updated";
            if (message is BookDeletedMessage)
                return "book.deleted";
            if (message is PersonCreatedMessage)
                return "person.created";
            if (message is PersonUpdatedMessage)
                return "person.updated";
            if (message is PersonDeletedMessage)
                return "person.deleted";

            return "default";
        }

        private IMessage CreateMessage(object entity, string messageType, string messageAction)
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
            else if (messageType == MessageTypeConstant.BookType && messageAction == MessageActionConstant.Update)
            {
                return new BookUpdatedMessage
                {
                    BookId = GetEntityId(entity),
                    BookData = entity as BookDTO,
                    Timestamp = DateTime.UtcNow,
                    CorrelationId = correlationId
                };
            }
            else if (messageType == MessageTypeConstant.PersonType && messageAction == MessageActionConstant.Create)
            {
                return new PersonCreatedMessage
                {
                    PersonId = GetEntityId(entity),
                    PersonData = entity as PersonDTO,
                    Timestamp = DateTime.UtcNow,
                    CorrelationId = correlationId
                };
            }
            else if (messageType == MessageTypeConstant.PersonType && messageAction == MessageActionConstant.Update)
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
            _channel?.Close();
            _connection?.Close();
        }
    }

    /// <summary>
    /// Messaging configuration settings
    /// </summary>
    public class MessagingSettings
    {
        public string ConnectionString { get; set; } = "amqp://localhost:5672";
        public string ExchangeName { get; set; } = "additional-service";
        public string ExchangeType { get; set; } = "topic";
        public string QueuePrefix { get; set; } = "additional-service";
        public int RetryCount { get; set; } = 3;
        public int RetryDelaySeconds { get; set; } = 5;
        public bool EnableMessaging { get; set; } = false;
    }
}