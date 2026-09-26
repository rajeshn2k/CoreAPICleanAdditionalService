namespace Core.Library.Clean.AdditionalService
{
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
}