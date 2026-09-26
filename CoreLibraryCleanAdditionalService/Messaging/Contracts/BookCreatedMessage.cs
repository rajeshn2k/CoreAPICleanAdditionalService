using Core.Library.Clean.AdditionalService;

namespace Core.Library.Clean.AdditionalService.Messaging.Contracts
{
    /// <summary>
    /// Message published when a book is created
    /// </summary>
    public class BookCreatedMessage : IMessage
    {
        public string BookId { get; set; }
        public BookDTO BookData { get; set; }
        public string MessageType => "BookCreated";
        public DateTime Timestamp { get; set; }
        public string CorrelationId { get; set; }
    }
}