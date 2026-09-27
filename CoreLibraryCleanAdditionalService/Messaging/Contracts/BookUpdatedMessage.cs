namespace Core.Library.Clean.AdditionalService
{
    /// <summary>
    /// Message published when a book is updated
    /// </summary>
    public class BookUpdatedMessage : IMessage
    {
        public string BookId { get; set; }
        public BookDTO BookData { get; set; }
        public string MessageType => "BookUpdated";
        public DateTime Timestamp { get; set; }
        public string CorrelationId { get; set; }
    }
}