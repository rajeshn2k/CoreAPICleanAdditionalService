namespace Core.Library.Clean.AdditionalService.Messaging.Contracts
{
    /// <summary>
    /// Message published when a book is deleted
    /// </summary>
    public class BookDeletedMessage : IMessage
    {
        public string BookId { get; set; }
        public string MessageType => "BookDeleted";
        public DateTime Timestamp { get; set; }
        public string CorrelationId { get; set; }
    }
}