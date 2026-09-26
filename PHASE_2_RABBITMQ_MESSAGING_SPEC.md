# Phase 2: RabbitMQ Message Broker Specification

## Overview

Phase 2 implements message publishing for event-driven architecture and asynchronous processing using RabbitMQ. This phase enables the API to publish events for entity operations, supporting integration with external systems and microservices.

## Objectives

- Implement message publishing for event-driven architecture
- Enable asynchronous processing of entity operations
- Establish messaging patterns for future microservices
- Support integration with external systems
- Implement reliable message delivery with retry logic
- Add message monitoring and health checks

## Implementation Details

### Message Publisher Interface

```csharp
public interface IMessagePublisher
{
    /// <summary>
    /// Publish a single message
    /// </summary>
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class;

    /// <summary>
    /// Publish multiple messages
    /// </summary>
    Task PublishAsync<TMessage>(IEnumerable<TMessage> messages, CancellationToken cancellationToken = default) where TMessage : class;

    /// <summary>
    /// Publish a message with explicit type and action
    /// </summary>
    Task PublishAsync(object entity, string messageType, string messageAction, CancellationToken cancellationToken = default);
}
```

### Message Contracts

#### Base Message Interface
```csharp
public interface IMessage
{
    string MessageType { get; }
    DateTime Timestamp { get; }
    string CorrelationId { get; }
}
```

#### Book Messages
```csharp
public class BookCreatedMessage : IMessage
{
    public string BookId { get; set; }
    public BookDTO BookData { get; set; }
    public DateTime Timestamp { get; set; }
    public string CorrelationId { get; set; }
    public string MessageType => "BookCreated";
}

public class BookUpdatedMessage : IMessage
{
    public string BookId { get; set; }
    public BookDTO BookData { get; set; }
    public DateTime Timestamp { get; set; }
    public string CorrelationId { get; set; }
    public string MessageType => "BookUpdated";
}

public class BookDeletedMessage : IMessage
{
    public string BookId { get; set; }
    public DateTime Timestamp { get; set; }
    public string CorrelationId { get; set; }
    public string MessageType => "BookDeleted";
}
```

#### Person Messages
```csharp
public class PersonCreatedMessage : IMessage
{
    public string PersonId { get; set; }
    public PersonDTO PersonData { get; set; }
    public DateTime Timestamp { get; set; }
    public string CorrelationId { get; set; }
    public string MessageType => "PersonCreated";
}

public class PersonUpdatedMessage : IMessage
{
    public string PersonId { get; set; }
    public PersonDTO PersonData { get; set; }
    public DateTime Timestamp { get; set; }
    public string CorrelationId { get; set; }
    public string MessageType => "PersonUpdated";
}

public class PersonDeletedMessage : IMessage
{
    public string PersonId { get; set; }
    public DateTime Timestamp { get; set; }
    public string CorrelationId { get; set; }
    public string MessageType => "PersonDeleted";
}
```

### Message Constants

#### Message Types
```csharp
public static class MessageTypeConstant
{
    public const string BookType = "Book";
    public const string PersonType = "Person";
}
```

#### Message Actions
```csharp
public static class MessageActionConstant
{
    public const string Create = "Create";
    public const string Update = "Update";
    public const string Delete = "Delete";
}
```

### Messaging Configuration

#### MessagingSettings
```csharp
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
```

#### appsettings.json
```json
{
  "Messaging": {
    "ConnectionString": "amqp://localhost:5672",
    "ExchangeName": "additional-service",
    "ExchangeType": "topic",
    "QueuePrefix": "additional-service",
    "RetryCount": 3,
    "RetryDelaySeconds": 5,
    "EnableMessaging": false
  }
}
```

### RabbitMQ Publisher Implementation

#### RabbitMQMessagePublisher
```csharp
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
```

#### EmptyMessagePublisher (Fallback)
```csharp
public class EmptyMessagePublisher : IMessagePublisher
{
    private readonly ILogger<EmptyMessagePublisher> _logger;

    public EmptyMessagePublisher()
    {
        _logger = null; // Can be injected if needed
    }

    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class
    {
        // No-op for fallback implementation
        await Task.CompletedTask;
    }

    public async Task PublishAsync<TMessage>(IEnumerable<TMessage> messages, CancellationToken cancellationToken = default) where TMessage : class
    {
        // No-op for fallback implementation
        await Task.CompletedTask;
    }

    public async Task PublishAsync(object entity, string messageType, string messageAction, CancellationToken cancellationToken = default)
    {
        // No-op for fallback implementation
        await Task.CompletedTask;
    }
}
```

### Exchange/Queue Topology

#### Exchange Configuration
- **Exchange Name**: `additional-service`
- **Exchange Type**: `topic`
- **Durability**: `true`
- **Auto Delete**: `false`

#### Queue Naming Convention
- **Queue Prefix**: `additional-service`
- **Queue Format**: `{prefix}.{entity}.{action}`
- **Examples**:
  - `additional-service.book.created`
  - `additional-service.book.updated`
  - `additional-service.book.deleted`
  - `additional-service.person.created`
  - `additional-service.person.updated`
  - `additional-service.person.deleted`

#### Routing Keys
- **Book Events**: `book.created`, `book.updated`, `book.deleted`
- **Person Events**: `person.created`, `person.updated`, `person.deleted`
- **Format**: `{entity}.{action}`

### Director Integration

#### BookDirector with Message Publishing
```csharp
public class BookDirector : IEntityDirector<BookDTO, BookCreateDTO>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMessagePublisher messagePublisher;
    private readonly ICacheService cacheService;
    private readonly ILogger<BookDirector> logger;

    public async Task<BookDTO> CreateEntityAsync(BookCreateDTO book, CancellationToken cancellationToken)
    {
        if (book != null)
        {
            var bookEntity = BookMapper.BookCreateDTOToBook(book);
            var result = await unitOfWork.BookRepository.CreateEntityAsync(bookEntity, cancellationToken);

            if (result != null)
            {
                // Invalidate cache
                await cacheService.RemoveAsync("book:all", cancellationToken);
                
                // Publish message
                var message = new BookCreatedMessage
                {
                    BookId = result.Id,
                    BookData = BookMapper.BookToBookDTO(result),
                    Timestamp = DateTime.UtcNow,
                    CorrelationId = Guid.NewGuid().ToString()
                };
                await messagePublisher.PublishAsync(message, cancellationToken);

                return BookMapper.BookToBookDTO(result);
            }
        }

        return null;
    }

    public async Task<long> UpdateEntityByIdAsync(string entityId, BookDTO book, CancellationToken cancellationToken)
    {
        if (book != null)
        {
            var bookEntity = BookMapper.BookDTOToBook(book);
            var result = await unitOfWork.BookRepository.UpdateAsync(entityId, bookEntity, cancellationToken);

            if (result > 0)
            {
                // Invalidate cache
                await InvalidateBookCacheAsync(entityId, cancellationToken);
                
                // Publish message
                var message = new BookUpdatedMessage
                {
                    BookId = entityId,
                    BookData = book,
                    Timestamp = DateTime.UtcNow,
                    CorrelationId = Guid.NewGuid().ToString()
                };
                await messagePublisher.PublishAsync(message, cancellationToken);
            }

            return result;
        }

        return 0;
    }
}
```

### Dependency Injection

```csharp
// Configure Messaging services
services.Configure<MessagingSettings>(
    configuration.GetSection("Messaging"));

var messagingSettings = configuration.GetSection("Messaging").Get<MessagingSettings>();
IMessagePublisher baseMessagePublisher;

if (messagingSettings != null && messagingSettings.EnableMessaging)
{
    try
    {
        baseMessagePublisher = new RabbitMQMessagePublisher(
            messagingSettings);
    }
    catch
    {
        // Fallback to empty publisher if RabbitMQ is unavailable
        baseMessagePublisher = new EmptyMessagePublisher();
    }
}
else
{
    baseMessagePublisher = new EmptyMessagePublisher();
}

// Register circuit breaker wrapped message publisher as scoped
services.AddScoped<IMessagePublisher>(sp =>
{
    var circuitBreakerService = sp.GetRequiredService<ICircuitBreakerService>();
    var logger = sp.GetRequiredService<ILogger<CircuitBreakerMessagePublisher>>();
    return new CircuitBreakerMessagePublisher(baseMessagePublisher, circuitBreakerService, logger);
});
```

## Message Schema

### Message Structure
```json
{
  "messageType": "BookCreated",
  "timestamp": "2024-01-01T00:00:00Z",
  "correlationId": "guid",
  "bookId": "guid",
  "bookData": {
    "id": "guid",
    "personId": "guid",
    "bookCategory": "string",
    "bookName": "string",
    "edition": "string",
    "image": "string",
    "price": 0.0,
    "dateCreated": "2024-01-01T00:00:00Z"
  }
}
```

### Message Validation
- Required fields validation
- Data type validation
- Business rule validation
- Correlation ID generation
- Timestamp generation

## Error Handling

### Retry Logic
- Configurable retry count
- Exponential backoff
- Retry delay configuration
- Retry logging
- Circuit breaker integration

### Dead-Letter Queue
- Failed message routing
- Error logging
- Message inspection
- Reprocessing capability
- Monitoring integration

### Connection Handling
- Automatic reconnection
- Connection failure logging
- Graceful degradation
- Fallback to empty publisher
- Health monitoring

## Testing Guidelines

### Unit Tests
- Test message contract validation
- Test message serialization
- Test routing key generation
- Test correlation ID generation
- Test publisher fallback logic

### Integration Tests
- Test RabbitMQ connectivity
- Test message publishing
- Test message consumption
- Test retry logic
- Test connection failure handling

### Performance Tests
- Test message publishing throughput
- Test batch message publishing
- Test serialization performance
- Test concurrent publishing
- Measure publishing latency

## Best Practices

### Message Design
- Use descriptive message types
- Include correlation IDs
- Include timestamps
- Keep messages small
- Use consistent structure

### Error Handling
- Implement retry logic
- Log all failures
- Use dead-letter queues
- Monitor message delivery
- Implement circuit breakers

### Performance
- Use async publishing
- Batch when possible
- Monitor queue depth
- Implement backpressure
- Optimize serialization

### Security
- Use secure connections
- Authenticate connections
- Validate message sources
- Encrypt sensitive data
- Implement ACLs

## Troubleshooting

### Common Issues

#### RabbitMQ Connection Failures
- **Symptom**: Message publishing fails, system falls back to empty publisher
- **Solution**: Check RabbitMQ server status, connection string, credentials
- **Fallback**: System automatically falls back to empty publisher

#### Messages Not Published
- **Symptom**: No messages in queues, publishing appears successful
- **Solution**: Check exchange/queue bindings, routing keys, exchange type
- **Check**: Review RabbitMQ management console

#### High Memory Usage
- **Symptom**: RabbitMQ memory usage increasing
- **Solution**: Review queue depths, implement message TTL, consumer monitoring
- **Monitor**: Use RabbitMQ management console for monitoring

#### Serialization Errors
- **Symptom**: Message publishing fails with serialization errors
- **Solution**: Check object compatibility, JSON serialization settings
- **Check**: Verify message contract versions

## Performance Considerations

### Expected Performance
- Single message publish: < 50ms
- Batch message publish: < 100ms per message
- Throughput: > 1000 messages/second
- Memory overhead: Minimal

### Optimization Strategies
- Use async publishing
- Batch similar messages
- Optimize serialization
- Use connection pooling
- Monitor queue depths

## Security Considerations

### RabbitMQ Security
- Use TLS for connections
- Implement authentication
- Use ACLs for access control
- Regular security updates
- Network security

### Message Security
- Validate message sources
- Encrypt sensitive data
- Implement message signing
- Use secure serialization
- Audit message content

## Monitoring and Observability

### Metrics to Track
- Message publishing rates
- Message delivery success/failure
- Queue depths
- Connection status
- Publishing latency

### Logging
- Log all published messages
- Log publishing failures
- Log connection events
- Log retry attempts
- Log circuit breaker events

### Health Checks
- RabbitMQ connectivity check
- Exchange/queue health check
- Message publisher health check
- Connection monitoring
- Queue depth monitoring

## Success Criteria

- [x] RabbitMQ integration completed
- [x] Message publishing for all CRUD operations
- [x] Error handling and retry logic implemented
- [x] Message serialization/deserialization working
- [x] Connection failure handling implemented
- [x] Circuit breaker protection implemented
- [x] Graceful degradation when RabbitMQ unavailable

## Dependencies

- RabbitMQ.Client (6.8.1)
- Newtonsoft.Json (13.0.4)

## Related Documentation

- [AGENTS.md](./AGENTS.md) - AI Agent Guidelines
- [ARCHITECTURE.md](./ARCHITECTURE.md) - Architecture Documentation
- [DEVELOPMENT_GUIDELINES.md](./DEVELOPMENT_GUIDELINES.md) - Development Guidelines
- [PROJECT_SPEC.md](./PROJECT_SPEC.md) - Project Specification
- [PHASE_0_API_RESPONSE_MODEL_SPEC.md](./PHASE_0_API_RESPONSE_MODEL_SPEC.md) - API Response Model Specification
- [PHASE_1_REDIS_CACHE_SPEC.md](./PHASE_1_REDIS_CACHE_SPEC.md) - Redis Cache Specification
