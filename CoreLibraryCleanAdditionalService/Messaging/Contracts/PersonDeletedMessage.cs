namespace Core.Library.Clean.AdditionalService.Messaging.Contracts
{
    /// <summary>
    /// Message published when a person is deleted
    /// </summary>
    public class PersonDeletedMessage : IMessage
    {
        public string PersonId { get; set; }
        public string MessageType => "PersonDeleted";
        public DateTime Timestamp { get; set; }
        public string CorrelationId { get; set; }
    }
}