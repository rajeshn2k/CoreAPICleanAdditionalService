using Core.Library.Clean.AdditionalService;

namespace Core.Library.Clean.AdditionalService.Messaging.Contracts
{
    /// <summary>
    /// Message published when a person is updated
    /// </summary>
    public class PersonUpdatedMessage : IMessage
    {
        public string PersonId { get; set; }
        public PersonDTO PersonData { get; set; }
        public string MessageType => "PersonUpdated";
        public DateTime Timestamp { get; set; }
        public string CorrelationId { get; set; }
    }
}