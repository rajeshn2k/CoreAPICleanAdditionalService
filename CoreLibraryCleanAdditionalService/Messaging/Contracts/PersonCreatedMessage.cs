using Core.Library.Clean.AdditionalService;

namespace Core.Library.Clean.AdditionalService.Messaging.Contracts
{
    /// <summary>
    /// Message published when a person is created
    /// </summary>
    public class PersonCreatedMessage : IMessage
    {
        public string PersonId { get; set; }
        public PersonDTO PersonData { get; set; }
        public string MessageType => "PersonCreated";
        public DateTime Timestamp { get; set; }
        public string CorrelationId { get; set; }
    }
}