namespace Core.Library.Clean.AdditionalService.Messaging.Contracts
{
    /// <summary>
    /// Base interface for all messages
    /// </summary>
    public interface IMessage
    {
        string MessageType { get; }
        DateTime Timestamp { get; }
        string CorrelationId { get; }
    }
}